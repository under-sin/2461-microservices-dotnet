using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using RestauranteService.Dtos;

namespace RestauranteService.RabbitMqClient;

public class RabbitMqClient : IRabbitMqClient
{
    private readonly IConfiguration _configuration;
    private readonly IConnection _connection;
    private readonly IModel _channel;

    public RabbitMqClient(IConfiguration configuration)
    {
        _configuration = configuration;
        _connection = new ConnectionFactory()
        {
            HostName = _configuration["RabbitMqHost"],
            Port = int.Parse(_configuration["RabbitMqPort"])
        }.CreateConnection();
        _channel = _connection.CreateModel();
        _channel.ExchangeDeclare(exchange: "trigger", type: ExchangeType.Fanout);
    }

    public void PublicaRestaurante(RestauranteReadDto restauranteReadDto)
    {
        string mensagem = JsonSerializer.Serialize(restauranteReadDto);
        byte[] body = Encoding.UTF8.GetBytes(mensagem);

        _channel.BasicPublish(
            exchange: "trigger",
            routingKey: "",
            basicProperties: null,
            body: body);
    }
}