using CarParkRulesEngine.Models;
using CarParkRulesEngine.Services;
using Microsoft.AspNetCore.Mvc;

namespace CarParkRulesEngine.Controllers;

public class EvaluationController(RuleEngine engine, IRuleRepository repo) : Controller
{
    private readonly RuleEngine _engine = engine;
    private readonly IRuleRepository _repo = repo;

    public IActionResult Index()
    {
        return View(new EvaluateViewModel());
    }

    [HttpPost]
    public async Task<IActionResult> Evaluate(EvaluateViewModel model)
    {
        var rules = await _repo.LoadRulesForCarPark(model.CarParkId);

        var ctx = new SessionContext
        {
            CarParkId = model.CarParkId,
            VRN = model.VRN,
            EntryTime = model.EntryTime,
            ExitTime = model.ExitTime,
            PreviousExitTime = await _repo.GetPreviousExit(model.VRN, model.CarParkId, model.EntryTime)
        };

        var result = _engine.Evaluate(ctx, rules);

        model.Result = result;
        model.SessionMinutes = ctx.SessionMinutes;
        model.AllowedMinutes = ctx.AllowedMinutes;

        return View("Result", model);
    }


    public async Task<IActionResult> AllResultsAsync(int carParkId = 1) //ste:todo: fixed to 1 car park for now, but could be made dynamic
    {
        var data = await _repo.LoadRulesAndSessions(carParkId);
        return View(data);
    }
}
