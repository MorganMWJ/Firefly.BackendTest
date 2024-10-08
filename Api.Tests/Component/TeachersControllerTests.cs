﻿using Api.DTOs;
using System.Net.Http.Json;
using System.Net;
using Microsoft.EntityFrameworkCore;
using Database;
using Microsoft.Extensions.DependencyInjection;

namespace Api.Tests.Component;

[Collection("ControllerTests")]
public class TeachersControllerTests
{
    private const string Endpoint = "api/teachers";

    private ControllerTestsFixture _fixture;

    public TeachersControllerTests(ControllerTestsFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task Created_for_valid_create_teacher_request()
    {
        // Arrange
        var httpJsonContent = JsonContent.Create(ValidCreateTeacherRequest);

        // Act
        var response = await _fixture.Client.PostAsync(Endpoint, httpJsonContent);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var responseObject = await response.Content.ReadFromJsonAsync<TeacherDto>();
        responseObject.Should().NotBeNull();
        responseObject!.Name.Should().Be(ValidCreateTeacherRequest.Name);
        responseObject!.Email.Should().Be(ValidCreateTeacherRequest.Email);

        _fixture.DbContextAccess(cxt =>
        {
            cxt.Teachers.Count(
                c => c.Name == ValidCreateTeacherRequest.Name &&
                c.Email == ValidCreateTeacherRequest.Email).Should().Be(1);
        });           
    }

    [Fact]
    public async Task Ok_returned_for_valid_teacher_id()
    {
        // Act
        var response = await _fixture.Client.GetAsync($"{Endpoint}/1");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var responseObject = await response.Content.ReadFromJsonAsync<TeacherDto>();
        responseObject.Should().NotBeNull();

        _fixture.DbContextAccess(cxt =>
        {
            var forAssert = cxt.Teachers.First(c => c.Id == 1);
            responseObject!.Name.Should().Be(forAssert.Name);
            responseObject!.Email.Should().Be(forAssert.Email);
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
    public async Task Ok_returned_for_valid_AssignClass_request()
    {
        // Arrange
        var teacherId = 1;
        var classId = 2;

        // Act
        var response = await _fixture.Client.PostAsync($"{Endpoint}/{teacherId}/classes/{classId}", null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        await _fixture.DbContextAccessAsync(async (cxt) =>
        {
            var teacher = await cxt.Teachers.FirstOrDefaultAsync(t => t.Id == teacherId);
            var cls = await cxt.Classes.FirstOrDefaultAsync(t => t.Id == classId);
            teacher.Classes.Should().ContainEquivalentOf(cls);
        });
    }

    [Fact]
    public async Task NotFount_returned_for_valid_missing_class_id()
    {
        // Arrange
        var teacherId = 1;
        var classId = 222;

        // Act
        var response = await _fixture.Client.PostAsync($"{Endpoint}/{teacherId}/classes/{classId}", null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task BadRequest_returned_for_teacher_with_max_classes()
    {
        // Arrange
        _fixture.DbContextAccess(_fixture.SeedTeacherWithMaxClasses);
        var teacherId = 4;
        var classId = 1;

        // Act
        var response = await _fixture.Client.PostAsync($"{Endpoint}/{teacherId}/classes/{classId}", null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    #region DataValidationTests

    [Fact]
    public async Task BadRequest_returned_for_name_greater_than_50_characters()
    {
        // Arrange
        var invalidRequest = ValidCreateTeacherRequest;
        invalidRequest.Name = _fixture.Faker.Random.String(51);
        var httpJsonContent = JsonContent.Create(invalidRequest);

        // Act
        var response = await _fixture.Client.PostAsync(Endpoint, httpJsonContent);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var responseString = await response.Content.ReadAsStringAsync();
        responseString.Should().Contain("must be a string or array type with a maximum length of '50'");
    }

    #endregion

    private TeacherDto ValidCreateTeacherRequest => new TeacherDto
    {
        Name = "Neil Taylor",
        Email = "nst@aber.ac.uk"
    };
}
