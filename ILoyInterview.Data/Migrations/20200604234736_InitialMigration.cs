using System;
using ILoyInterview.Data.Entities.Enums;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ILoyInterview.Data.Migrations
{
    public partial class InitialMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql($@"
                CREATE FUNCTION dbo.GetProjectState(@projectId int)
                  RETURNS INT
                AS
                BEGIN
                    DECLARE @totalTaskCount int;
                    DECLARE @completedTaskCount int;
                    DECLARE @inProgressTaskCount int;
                    DECLARE @resultState int;

                    DECLARE @stateRusults TABLE(
                        [Count] int NOT NULL,
                        [State] int NOT NULL
                    );

                    WITH ProjectCte as
                    (
                      SELECT p.Id, p.ParentProjectId
                      FROM Projects p
                      WHERE p.Id = @projectId

                      UNION ALL

                      SELECT p.Id, P.ParentProjectId
                      FROM Projects P
                      INNER JOIN ProjectCte c
                      ON c.Id = p.ParentProjectId
                    ) 

                    INSERT INTO @stateRusults ([Count], [State])
                        SELECT COUNT(t.Id), t.[State]
                        FROM ProjectCte p
                            INNER JOIN ProjectTasks t ON t.ProjectId = p.Id
                        GROUP BY t.[State]

                    SET @totalTaskCount = (SELECT SUM([Count]) FROM @stateRusults)
                    SET @completedTaskCount = (SELECT TOP 1 [Count] FROM @stateRusults WHERE [State] = {StateEnum.Completed})

                    -- beter to do this select only if @completedTaskCount check fails
                    SET @inProgressTaskCount = (SELECT TOP 1 [Count] FROM @stateRusults WHERE [State] = {StateEnum.InProgress})

                    BEGIN
                        -- assume that Project with no tasks is planned
                        IF @totalTaskCount <> 0 AND @totalTaskCount = @completedTaskCount
                            SET @resultState = {StateEnum.Completed}
                        ELSE IF @inProgressTaskCount > 0
                            SET @resultState = {StateEnum.InProgress}
                        ELSE 
                            SET @resultState = {StateEnum.Planned}
                    END

                    RETURN @resultState
                END;
                ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP FUNCTION dbo.GetProjectState");
        }
    }
}
