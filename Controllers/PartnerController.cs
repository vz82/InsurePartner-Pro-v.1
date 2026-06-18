using InsurancePartnerApp.Models;
using InsurancePartnerApp.Services;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;

namespace InsurancePartnerApp.Controllers;

public class PartnerController : Controller
{
    private readonly IPartnerService _partnerService;
    private readonly IPolicyService _policyService;

    public PartnerController(IPartnerService partnerService, IPolicyService policyService)
    {
        _partnerService = partnerService;
        _policyService = policyService;
    }

    public async Task<IActionResult> Index()
    {
        var partners = await _partnerService.GetAllPartnersAsync();
        return View(partners);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Partner model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        // Validate FirstName
        if (string.IsNullOrWhiteSpace(model.FirstName) || model.FirstName.Length < 2 || model.FirstName.Length > 255)
            ModelState.AddModelError(nameof(model.FirstName), "FirstName must be between 2 and 255 characters");

        // Validate LastName
        if (string.IsNullOrWhiteSpace(model.LastName) || model.LastName.Length < 2 || model.LastName.Length > 255)
            ModelState.AddModelError(nameof(model.LastName), "LastName must be between 2 and 255 characters");

        // Validate PartnerNumber (exactly 20 digits)
        if (string.IsNullOrWhiteSpace(model.PartnerNumber) || !Regex.IsMatch(model.PartnerNumber, @"^\d{20}$"))
            ModelState.AddModelError(nameof(model.PartnerNumber), "PartnerNumber must be exactly 20 digits");

        // Check if PartnerNumber already exists
        if (!string.IsNullOrWhiteSpace(model.PartnerNumber) && await _partnerService.PartnerNumberExistsAsync(model.PartnerNumber))
            ModelState.AddModelError(nameof(model.PartnerNumber), "This PartnerNumber already exists");

        // Validate ExternalCode
        if (string.IsNullOrWhiteSpace(model.ExternalCode) || model.ExternalCode.Length < 10 || model.ExternalCode.Length > 20)
            ModelState.AddModelError(nameof(model.ExternalCode), "ExternalCode must be between 10 and 20 characters");

        // Check if ExternalCode already exists
        if (!string.IsNullOrWhiteSpace(model.ExternalCode) && await _partnerService.ExternalCodeExistsAsync(model.ExternalCode))
            ModelState.AddModelError(nameof(model.ExternalCode), "This ExternalCode already exists");

        // Validate CreatedByUser (email format)
        if (string.IsNullOrWhiteSpace(model.CreatedByUser) || model.CreatedByUser.Length > 255 || !Regex.IsMatch(model.CreatedByUser, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            ModelState.AddModelError(nameof(model.CreatedByUser), "CreatedByUser must be a valid email address");

        // Validate Gender
        if (!new[] { 'M', 'F', 'N' }.Contains(model.Gender))
            ModelState.AddModelError(nameof(model.Gender), "Gender must be M, F, or N");

        // Validate PartnerTypeId
        if (model.PartnerTypeId != 1 && model.PartnerTypeId != 2)
            ModelState.AddModelError(nameof(model.PartnerTypeId), "PartnerTypeId must be 1 (Personal) or 2 (Legal)");

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            var id = await _partnerService.CreatePartnerAsync(model);
            TempData["SuccessId"] = id;
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, $"Error creating partner: {ex.Message}");
            return View(model);
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetPartnerDetails(int id)
    {
        var partner = await _partnerService.GetPartnerByIdAsync(id);
        if (partner == null)
            return NotFound();

        var policies = await _policyService.GetPoliciesByPartnerIdAsync(id);
        
        return Json(new
        {
            partner = new
            {
                partner.Id,
                partner.FullName,
                partner.Address,
                partner.PartnerNumber,
                partner.CroatianPIN,
                PartnerType = partner.PartnerTypeId == 1 ? "Personal" : "Legal",
                partner.CreatedAtUtc,
                partner.CreatedByUser,
                partner.IsForeign,
                partner.ExternalCode,
                partner.Gender,
                partner.PolicyCount,
                partner.TotalPolicyAmount,
                partner.HasHighRisk
            },
            policies = policies.Select(p => new
            {
                p.ShelfNumber,
                p.PolicyAmount
            })
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddPolicy(int partnerId, string shelfNumber, decimal policyAmount)
    {
        // Validate ShelfNumber
        if (string.IsNullOrWhiteSpace(shelfNumber) || shelfNumber.Length < 10 || shelfNumber.Length > 15)
        {
            return Json(new { success = false, message = "ShelfNumber must be between 10 and 15 characters" });
        }

        // Validate PolicyAmount
        if (policyAmount <= 0)
        {
            return Json(new { success = false, message = "PolicyAmount must be greater than 0" });
        }

        try
        {
            var policy = new Policy
            {
                PartnerId = partnerId,
                ShelfNumber = shelfNumber,
                PolicyAmount = policyAmount
            };

            await _policyService.CreatePolicyAsync(policy);
            return Json(new { success = true, message = "Policy added successfully" });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = $"Error: {ex.Message}" });
        }
    }
}