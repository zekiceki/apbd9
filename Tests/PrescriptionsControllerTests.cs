using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MedicalPrescriptionAPI.Controllers;
using MedicalPrescriptionAPI.Data;
using MedicalPrescriptionAPI.Models;
using Xunit;

namespace MedicalPrescriptionAPI.Tests
{
    public class PrescriptionsControllerTests
    {
        [Fact]
        public async Task AddPrescription_ShouldReturnBadRequest_WhenDoctorNotFound()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            using var context = new AppDbContext(options);
            var controller = new PrescriptionsController(context);

            var prescriptionDto = new PrescriptionDto
            {
                PatientId = 1,
                PatientFirstName = "John",
                PatientLastName = "Doe",
                PatientBirthdate = DateTime.Now,
                DoctorId = 1,
                Date = DateTime.Now,
                DueDate = DateTime.Now.AddDays(10),
                Medicaments = new List<MedicamentDto>
                {
                    new MedicamentDto { MedicamentId = 1, Dose = 10, Description = "Test" }
                }
            };

            // Act
            var result = await controller.AddPrescription(prescriptionDto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Doctor not found", badRequestResult.Value);
        }
    }
}