using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MedicalPrescriptionAPI.Data;
using MedicalPrescriptionAPI.Models;

namespace MedicalPrescriptionAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PrescriptionsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public PrescriptionsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> AddPrescription([FromBody] PrescriptionDto prescriptionDto)
        {
            var patient = await _context.Patients.FindAsync(prescriptionDto.PatientId) ?? new Patient
            {
                Id = prescriptionDto.PatientId,
                FirstName = prescriptionDto.PatientFirstName,
                LastName = prescriptionDto.PatientLastName,
                Birthdate = prescriptionDto.PatientBirthdate
            };

            if (patient.Id == 0)
                _context.Patients.Add(patient);

            var doctor = await _context.Doctors.FindAsync(prescriptionDto.DoctorId);
            if (doctor == null)
                return BadRequest("Doctor not found");

            if (prescriptionDto.Medicaments.Count > 10)
                return BadRequest("A prescription can include a maximum of 10 medications");

            foreach (var med in prescriptionDto.Medicaments)
            {
                var medicament = await _context.Medicaments.FindAsync(med.MedicamentId);
                if (medicament == null)
                    return BadRequest($"Medicament with Id {med.MedicamentId} not found");
            }

            var prescription = new Prescription
            {
                Date = prescriptionDto.Date,
                DueDate = prescriptionDto.DueDate,
                Patient = patient,
                Doctor = doctor,
                PrescriptionMedicaments = prescriptionDto.Medicaments.Select(m => new PrescriptionMedicament
                {
                    MedicamentId = m.MedicamentId,
                    Dose = m.Dose,
                    Description = m.Description
                }).ToList()
            };

            _context.Prescriptions.Add(prescription);
            await _context.SaveChangesAsync();

            return Ok(prescription);
        }
    }
}