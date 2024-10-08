using System.Net.Http.Json;
using System.Net;
using Api.DTOs;
using Bogus;

namespace Api.Tests.Component;

[Collection("ControllerTests")]
public class StudentsControllerTests
{
    private const string Endpoint = "api/students";

    private ControllerTestsFixture _fixture;

    public StudentsControllerTests(ControllerTestsFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task Created_for_valid_create_student_request()
    {
        // Arrange
        var httpJsonContent = JsonContent.Create(ValidCreateStudentRequest);

        // Act
        var response = await _fixture.Client.PostAsync(Endpoint, httpJsonContent);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var responseObject = await response.Content.ReadFromJsonAsync<StudentDto>();
        responseObject.Should().NotBeNull();
        responseObject!.Name.Should().Be(ValidCreateStudentRequest.Name);
        responseObject!.Email.Should().Be(ValidCreateStudentRequest.Email);

        _fixture.DbContext.Students.Count(
            c => c.Name == ValidCreateStudentRequest.Name &&
            c.Email == ValidCreateStudentRequest.Email).Should().Be(1);
    }

    [Fact]
    public async Task Ok_returned_for_valid_student_id()
    {
        // Act
        var response = await _fixture.Client.GetAsync($"{Endpoint}/3");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var responseObject = await response.Content.ReadFromJsonAsync<StudentDto>();
        responseObject.Should().NotBeNull();

        var forAssert = _fixture.DbContext.Students.First(c => c.Id == 3);
        responseObject!.Name.Should().Be(forAssert.Name);
        responseObject!.Email.Should().Be(forAssert.Email);
    }

    #region DataValidationTests

    [Fact]
    public async Task BadRequest_returned_for_name_greater_than_50_characters()
    {
        // Arrange
        var invalidRequest = ValidCreateStudentRequest;
        invalidRequest.Name = _fixture.Faker.Random.String(51);
        var httpJsonContent = JsonContent.Create(invalidRequest);

        // Act
        var response = await _fixture.Client.PostAsync(Endpoint, httpJsonContent);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var responseString = await response.Content.ReadAsStringAsync();
        responseString.Should().Contain("must be a string or array type with a maximum length of '50'");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public async Task BadRequest_returned_for_missing_name(string? name)
    {
        // Arrange
        var invalidRequest = ValidCreateStudentRequest;
        invalidRequest.Name = name;
        var httpJsonContent = JsonContent.Create(invalidRequest);

        // Act
        var response = await _fixture.Client.PostAsync(Endpoint, httpJsonContent);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var responseString = await response.Content.ReadAsStringAsync();
        responseString.Should().Contain("The Name field is required.");
    }

    #endregion

    private StudentDto ValidCreateStudentRequest => new StudentDto 
    {
        Name = "John Smith",
        Email = "john.smith@test.ac.uk"
    };

}
