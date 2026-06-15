using LiteDB;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TaskManagementSystem.Application.Abstractions;
using TaskManagementSystem.Domain.Entities;
using TaskManagementSystem.Domain.Enums;

namespace TaskManagementSystem.Infrastructure.Database;

public sealed class ManagementSeedDataInitializer
{
    private const string SeedPassword = "1234567890";
    private static readonly DateTime SeedBaseDateUtc = new(2026, 7, 1, 9, 0, 0, DateTimeKind.Utc);
    private readonly IPasswordHasher _passwordHasher;
    private readonly ILogger<ManagementSeedDataInitializer> _logger;
    private readonly LiteDbSettings _settings;
    private readonly IServiceProvider _serviceProvider;

    public ManagementSeedDataInitializer(
        IServiceProvider serviceProvider,
        IOptionsSnapshot<LiteDbSettings> settingsOptions,
        IPasswordHasher passwordHasher,
        ILogger<ManagementSeedDataInitializer> logger)
    {
        _serviceProvider = serviceProvider;
        _settings = settingsOptions.Value;
        _passwordHasher = passwordHasher;
        _logger = logger;
    }

    public async Task InitializeAsync()
    {
        var databasePath = ResolveDatabasePath(_settings.ConnectionString);

        if (databasePath is not null && File.Exists(databasePath))
        {
            _logger.LogInformation("Skipping seed initialization because database already exists at {DatabasePath}.", databasePath);
            return;
        }

        if (!string.IsNullOrWhiteSpace(databasePath))
        {
            var directoryPath = Path.GetDirectoryName(databasePath);

            if (!string.IsNullOrWhiteSpace(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }
        }

        var userRepository = _serviceProvider.GetRequiredService<IManagementUserRepository>();
        var taskRepository = _serviceProvider.GetRequiredService<IManagementTaskRepository>();
        var users = CreateSeedUsers();
        var tasks = CreateSeedTasks(users);

        foreach (var user in users)
        {
            await userRepository.CreateAsync(user);
        }

        foreach (var task in tasks)
        {
            await taskRepository.CreateAsync(task);
        }

        _logger.LogInformation(
            "Seeded database with {UserCount} management users and {TaskCount} management tasks.",
            users.Count,
            tasks.Count);
    }

    internal static string? ResolveDatabasePath(string connectionString)
    {
        var parsed = new ConnectionString(connectionString);

        if (string.IsNullOrWhiteSpace(parsed.Filename))
        {
            return null;
        }

        return Path.IsPathRooted(parsed.Filename)
            ? parsed.Filename
            : Path.GetFullPath(parsed.Filename, Directory.GetCurrentDirectory());
    }

    private List<ManagementUser> CreateSeedUsers()
    {
        var sharedPasswordHash = _passwordHasher.HashPassword(SeedPassword);

        return
        [
            new ManagementUser(CreateGuid(1), "Alicia Moreno", "alicia.moreno@example.com", sharedPasswordHash),
            new ManagementUser(CreateGuid(2), "Marcus Bennett", "marcus.bennett@example.com", sharedPasswordHash),
            new ManagementUser(CreateGuid(3), "Sofia Chen", "sofia.chen@example.com", sharedPasswordHash)
        ];
    }

    private static List<ManagementTask> CreateSeedTasks(IReadOnlyList<ManagementUser> users)
    {
        return
        [
            .. BuildTasksForUser(
                users[0].Id,
                1001,
                [
                    ("Finalize Q3 staffing plan", "Review open roles, adjust hiring targets, and confirm the final staffing proposal with department leads."),
                    ("Prepare onboarding checklist refresh", "Update onboarding tasks to reflect the latest security, tooling, and documentation requirements."),
                    ("Consolidate support handoff notes", "Gather recurring customer issues and convert them into a clear handoff summary for the support team."),
                    ("Review vendor renewal calendar", "Audit contract renewal dates and flag agreements requiring early approval or renegotiation."),
                    ("Draft leadership meeting agenda", "Prepare discussion topics, decisions needed, and follow-up owners for the weekly leadership sync."),
                    ("Coordinate workspace move plan", "List logistics, dependencies, and communication steps needed for the office seating reshuffle."),
                    ("Validate monthly budget assumptions", "Cross-check planned expenses against current forecasts and identify unexpected variances."),
                    ("Refresh incident response contacts", "Verify that emergency contacts, escalation paths, and role assignments remain current."),
                    ("Document release readiness criteria", "Summarize the release gate checklist used by product, QA, and operations before deployment."),
                    ("Schedule cross-team planning session", "Organize a planning workshop to align delivery risks, blockers, and milestone owners."),
                    ("Update performance review tracker", "Bring the review schedule and completion tracker up to date for all active contributors."),
                    ("Create quarterly training outline", "Prepare the structure and proposed sessions for the next internal training cycle."),
                    ("Review audit evidence inventory", "Check that required audit artifacts are available, named consistently, and easy to retrieve."),
                    ("Align backlog cleanup priorities", "Identify stale backlog items and propose a cleanup order with product stakeholders."),
                    ("Prepare remote access policy recap", "Summarize current remote access expectations and highlight policy updates for the team."),
                    ("Organize customer escalation log", "Reformat recent escalations into a clear log with severity, owner, and next action."),
                    ("Review maintenance window notice", "Draft and validate the communication that will be sent before the planned maintenance window."),
                    ("Track open procurement requests", "Compile outstanding procurement items and confirm approvals still required for each one."),
                    ("Summarize retrospectives action items", "Capture action items from the latest retrospectives and group them by owner."),
                    ("Plan documentation ownership update", "Reassign documentation sections so each critical area has a clear accountable owner."),
                    ("Review security awareness timeline", "Confirm campaign milestones, reminders, and completion targets for the next awareness cycle."),
                    ("Prepare customer visit itinerary", "Outline the agenda, logistics, and preparation items for the upcoming customer onsite."),
                    ("Consolidate analytics dashboard feedback", "Gather stakeholder feedback on the dashboard and group requests by urgency."),
                    ("Update team availability calendar", "Normalize upcoming leave, travel, and coverage information for planning purposes."),
                    ("Check license usage summary", "Compare assigned licenses against active usage and flag obvious optimization opportunities.")
                ]),
            .. BuildTasksForUser(
                users[1].Id,
                2001,
                [
                    ("Prepare finance follow-up list", "Capture finance review questions and assign owners for each unresolved budget item."),
                    ("Review operational risk register", "Inspect the current risk register and refresh mitigation notes for overdue entries."),
                    ("Draft customer launch briefing", "Create a concise launch brief with dependencies, open issues, and communication timing."),
                    ("Confirm legal approval tracker", "Verify that pending legal reviews are still on schedule and update missing owners."),
                    ("Schedule platform health review", "Plan a review meeting focused on recurring incidents and service stability trends."),
                    ("Refresh internal FAQ article", "Rewrite outdated answers in the internal FAQ based on the latest support feedback."),
                    ("Review partnership commitments", "List promised deliverables for active partners and validate upcoming deadlines."),
                    ("Prepare monthly ops digest", "Assemble the monthly operations summary for leadership and functional leads."),
                    ("Align test environment requests", "Collect environment requests from teams and sequence them based on urgency."),
                    ("Organize marketing asset review", "Prepare the checklist and schedule for approving the next set of campaign assets."),
                    ("Update internal training attendance", "Normalize attendee records and highlight incomplete training follow-ups.")
                ]),
            .. BuildTasksForUser(
                users[2].Id,
                3001,
                [
                    ("Prepare design review recap", "Summarize the latest design review decisions and call out unresolved follow-up items."),
                    ("Review accessibility fixes backlog", "Inspect pending accessibility improvements and confirm which ones unblock upcoming releases."),
                    ("Draft stakeholder update note", "Write the short stakeholder note that summarizes progress, risks, and next steps."),
                    ("Organize usability interview notes", "Group recent usability findings into themes for easier prioritization."),
                    ("Validate content publication checklist", "Confirm the publication checklist covers approvals, assets, and final QA steps.")
                ])
        ];
    }

    private static IEnumerable<ManagementTask> BuildTasksForUser(
        Guid userId,
        int startingIdentifier,
        IReadOnlyList<(string Title, string Description)> entries)
    {
        for (var index = 0; index < entries.Count; index++)
        {
            var (title, description) = entries[index];
            var status = (index % 3) switch
            {
                0 => ManagementTaskStatus.Pending,
                1 => ManagementTaskStatus.InProgress,
                _ => ManagementTaskStatus.Completed
            };
            var dueDate = SeedBaseDateUtc.AddDays(index * 3).AddHours(index % 5);

            yield return new ManagementTask(
                CreateGuid(startingIdentifier + index),
                title,
                description,
                status,
                dueDate,
                userId);
        }
    }

    private static Guid CreateGuid(int value)
    {
        return Guid.Parse($"00000000-0000-0000-0000-{value:000000000000}");
    }
}
