var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.PunchInAndOut>("punchinandout");

builder.AddProject<Projects.LineNotifyApi>("linenotifyapi");

builder.Build().Run();
