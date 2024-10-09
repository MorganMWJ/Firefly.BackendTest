using System.Net.Http.Json;
using System.Net;
using Api.DTOs;
using Microsoft.EntityFrameworkCore;

namespace Api.Tests.Component;

[Collection("ControllerTests")]
public class ClassesControllerTests
{
    private const string Endpoint = "api/classes";

    private ControllerTestsFixture _fixture;

    public ClassesControllerTests(ControllerTestsFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task Created_for_valid_create_class_request()
    {
        // Arrange
        var httpJsonContent = JsonContent.Create(ValidCreateClassRequest);

        // Act
        var response = await _fixture.Client.PostAsync(Endpoint, httpJsonContent);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var responseObject = await response.Content.ReadFromJsonAsync<ClassDto>();
        responseObject.Should().NotBeNull();
        responseObject!.Name.Should().Be(ValidCreateClassRequest.Name);
        responseObject!.Capacity.Should().Be(ValidCreateClassRequest.Capacity);

        _fixture.DbContextAccess(cxt =>
        {
            cxt.Classes.Count(
                c => c.Name == ValidCreateClassRequest.Name &&
                c.Capacity == ValidCreateClassRequest.Capacity).Should().Be(1);
        });            
    }

    [Fact]
    public async Task Ok_returned_for_valid_class_id()
    {
        // Act
        var response = await _fixture.Client.GetAsync($"{Endpoint}/1");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var responseObject = await response.Content.ReadFromJsonAsync<ClassDto>();
        responseObject.Should().NotBeNull();

        _fixture.DbContextAccess(cxt =>
        {
            var clsForAssert = cxt.Classes.First(c => c.Id == 1);
            responseObject!.Name.Should().Be(clsForAssert.Name);
            responseObject!.Capacity.Should().Be(clsForAssert.Capacity);
        });        
    }

    [Fact]
    public async Task NotFound_returned_for_invalid_id()
    {
        // Act
        var response = await _fixture.Client.GetAsync($"{Endpoint}/51");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Ok_returned_for_valid_EnrollStudent_request()
    {
        // Arrange
        var studentId = 1;
        var classId = 2;

        // Act
        var response = await _fixture.Client.PostAsync($"{Endpoint}/{classId}/students/{studentId}", null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        await _fixture.DbContextAccessAsync(async (cxt) =>
        {
            var student = await cxt.Students.Include(s=>s.Classes).FirstAsync(s => s.Id == studentId);
            var cls = await cxt.Classes.Include(c=>c.Students).FirstAsync(s => s.Id == classId);

            student.Classes.Should().ContainEquivalentOf(cls);
            cls.Students.Should().ContainEquivalentOf(student);
        });
    }

    [Fact]
    public async Task NotFount_returned_for_valid_missing_class_id()
    {
        // Arrange
        var studentId = 1;
        var classId = 233;

        // Act
        var response = await _fixture.Client.PostAsync($"{Endpoint}/{classId}/students/{studentId}", null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task BadRequest_returned_for_over_capacity_class()
    {
        // Arrange
        var studentId = 1;
        var classId = 2;
        _fixture.SeedOverCapacityClass(classId);

        // Act
        var response = await _fixture.Client.PostAsync($"{Endpoint}/{classId}/students/{studentId}", null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    #region DataValidationTests

    [Fact]
    public async Task BadRequest_returned_for_name_greater_than_20_characters()
    {
        // Arrange
        var invalidCreateClassRequest = ValidCreateClassRequest;
        invalidCreateClassRequest.Name = _fixture.Faker.Random.String(51);
        var httpJsonContent = JsonContent.Create(invalidCreateClassRequest);

        // Act
        var response = await _fixture.Client.PostAsync(Endpoint, httpJsonContent);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var responseString = await response.Content.ReadAsStringAsync();
        responseString.Should().Contain("must be a string or array type with a maximum length of '20'");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public async Task BadRequest_returned_for_missing_name(string? name)
    {
        // Arrange
        var invalidCreateClassRequest = ValidCreateClassRequest;
        invalidCreateClassRequest.Name = name;
        var httpJsonContent = JsonContent.Create(invalidCreateClassRequest);

        // Act
        var response = await _fixture.Client.PostAsync(Endpoint, httpJsonContent);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var responseString = await response.Content.ReadAsStringAsync();
        responseString.Should().Contain("The Name field is required.");
    }

    [Theory]
    [InlineData(4)]
    [InlineData(51)]
    public async Task BadRequest_returned_for_capacity_out_of_range_5_to_30(int invalidCapcity)
    {
        // Arrange
        var invalidCreateClassRequest = ValidCreateClassRequest;
        invalidCreateClassRequest.Capacity = invalidCapcity;
        var httpJsonContent = JsonContent.Create(invalidCreateClassRequest);

        // Act
        var response = await _fixture.Client.PostAsync(Endpoint, httpJsonContent);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var responseString = await response.Content.ReadAsStringAsync();
        responseString.Should().Contain("The field Capacity must be between 5 and 50.");
    }

    #endregion

    private ClassDto ValidCreateClassRequest => new ClassDto
    {
        Name = "Spanish",
        Capacity = 30
    };
}