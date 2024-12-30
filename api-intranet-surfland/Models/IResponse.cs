namespace api_intranet_surfland.Models; 
public class IResponse {
    public bool status { get; set; }
    public string? message { get; set; } = "Operação realizada com sucesso";
    public object? data { get; set; } = new List<Object>();
}
