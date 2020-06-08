namespace ILoyInterview.Contracts.ApiModels.Common
{
    public class ResponseWrapperModel<TDataModel>
    {
        public TDataModel Data { get; set; }

        public bool Success { get; set; }

        public string Error { get; set; }
    }
}
