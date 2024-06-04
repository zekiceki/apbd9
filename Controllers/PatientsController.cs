using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MedicalPrescriptionAPI.Data;
using MedicalPrescriptionAPI.Models;

namespace MedicalPrescriptionAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PatientsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public PatientsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPatient(int id)
        {
            var patient = await _context.Patients
                .Include(p => p.Prescriptions)
                    .ThenInclude(p => p.PrescriptionMedicaments)
                        .ThenInclude(pm => pm.Medicament)
                .Include(p => p.Prescriptions)
                    .ThenInclude(p => p.Doctor)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (patient == null)
                return NotFound();

            var result = new
            {
                patient.Id,
                patient.FirstName,
                patient.LastName,
                patient.Birthdate,
                Prescriptions = patient.Prescriptions.Select(p => new
                {
                    p.Id,
                    p.Date,
                    p.DueDate,
                    Medicaments = p.PrescriptionMedicaments.Select(pm => new
                    {
                        pm.Medicament.Id,
                        pm.Medicament.Name,
                        pm.Dose,
                        pm.Description
                    }),
                    Doctor = new
                    {
                        p.Doctor.Id,
                        p.Doctor.FirstName,
                        p.Doctor.LastName,
                        p.Doctor.Specialization
                    }
                })
            };

            return Ok(result);
        }
    }
}