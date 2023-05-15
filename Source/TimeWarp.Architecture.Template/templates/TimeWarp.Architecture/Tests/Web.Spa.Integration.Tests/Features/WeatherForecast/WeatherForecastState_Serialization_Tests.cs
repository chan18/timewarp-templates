namespace WeatherForecastDto_;

using FluentAssertions;
using System;
using System.Text.Json;
using TimeWarp.Architecture.Features.WeatherForecasts;

public class Should
{
  public void SerializeAndDeserialize()
  {
    //Arrange
    var jsonSerializerOptions = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
    var weatherForecastDto = new WeatherForecastDto
    (
      date: DateTime.MinValue.ToUniversalTime(),
      summary: "Summary 1",
      temperatureC: 24
    );

    string json = JsonSerializer.Serialize(weatherForecastDto, jsonSerializerOptions);

    //Act
    WeatherForecastDto parsed = JsonSerializer.Deserialize<WeatherForecastDto>(json, jsonSerializerOptions);

    //Assert
    weatherForecastDto.TemperatureC.Should().Be(parsed.TemperatureC);
    weatherForecastDto.Summary.Should().Be(parsed.Summary);
    weatherForecastDto.Date.Should().Be(parsed.Date);
  }
}
