using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Served.SDK.Client.Core;
using Served.SDK.Models.Common;
using Served.SDK.Models.Projects;
using Served.SDK.Models.Tasks;

namespace Served.SDK.Client.Apis;

/// <summary>
/// API module for project management resources including projects, tasks, and sprints.
/// </summary>
public class ProjectManagementApi : ApiModuleBase
{
    protected override string ModulePath => "project-management";

    /// <summary>
    /// Access to project resources.
    /// </summary>
    public ProjectsResource Projects { get; }

    /// <summary>
    /// Access to task resources.
    /// </summary>
    public TasksResource Tasks { get; }

    public ProjectManagementApi(IHttpClient http) : base(http)
    {
        Projects = new ProjectsResource(http, this);
        Tasks = new TasksResource(http, this);
    }

    #region Projects Resource

    /// <summary>
    /// Resource client for project operations.
    /// </summary>
    public class ProjectsResource : BulkApiClientBase<
        ProjectSummary,
        ProjectDetail,
        CreateProjectRequest,
        UpdateProjectRequest,
        ProjectQueryParams,
        BulkCreateProjectsRequest,
        BulkUpdateProjectsRequest,
        BulkDeleteProjectsRequest>
    {
        private readonly ProjectManagementApi _module;

        // Use legacy path for backwards compatibility (api/projects)
        protected override string BasePath => "api/projects";

        internal ProjectsResource(IHttpClient http, ProjectManagementApi module) : base(http)
        {
            _module = module;
        }

        protected override IEnumerable<string> GetCustomQueryParams(ProjectQueryParams query)
        {
            var @params = new List<string>();

            if (query.CustomerId.HasValue)
                @params.Add($"customerId={query.CustomerId.Value}");
            if (query.ProjectStatusId.HasValue)
                @params.Add($"projectStatusId={query.ProjectStatusId.Value}");
            if (query.IsActive.HasValue)
                @params.Add($"isActive={query.IsActive.Value}");
            if (query.ParentId.HasValue)
                @params.Add($"parentId={query.ParentId.Value}");

            return @params;
        }

        protected override List<ProjectSummary>? MapToEntityList(List<ProjectDetail>? details)
        {
            return details?.Select(d => new ProjectSummary
            {
                Id = d.Id,
                Name = d.Name,
                ProjectNo = d.ProjectNo,
                StartDate = d.StartDate,
                EndDate = d.EndDate,
                ProjectStatusId = d.ProjectStatusId,
                CustomerId = d.CustomerId,
                ProjectManagerId = d.ProjectManagerId,
                IsActive = d.IsActive
            }).ToList();
        }

        /// <summary>
        /// Gets sub-projects of a parent project.
        /// </summary>
        public Task<List<ProjectSummary>> GetSubProjectsAsync(int parentId)
        {
            return GetAllAsync(new ProjectQueryParams { ParentId = parentId });
        }

        /// <summary>
        /// Searches projects by term.
        /// </summary>
        public Task<List<ProjectSummary>> SearchAsync(string searchTerm, int take = 20)
        {
            return GetAllAsync(new ProjectQueryParams
            {
                Search = searchTerm,
                Take = take,
                IsActive = true
            });
        }

        /// <summary>
        /// Gets projects for a specific customer.
        /// </summary>
        public async Task<List<ProjectSummary>> GetByCustomerAsync(int customerId, int take = 100)
        {
            var response = await Http.GetAsync<ApiV2ListResponse<ProjectDetail>>(
                $"api/projects/by-customer/{customerId}?pageSize={take}");

            return response.Data?.Select(d => new ProjectSummary
            {
                Id = d.Id,
                Name = d.Name,
                ProjectNo = d.ProjectNo,
                StartDate = d.StartDate,
                EndDate = d.EndDate,
                ProjectStatusId = d.ProjectStatusId,
                CustomerId = d.CustomerId,
                ProjectManagerId = d.ProjectManagerId,
                IsActive = d.IsActive
            }).ToList() ?? new List<ProjectSummary>();
        }

        /// <summary>
        /// Gets multiple projects by IDs.
        /// </summary>
        public async Task<List<ProjectDetail>> GetRangeAsync(List<int> ids)
        {
            var projects = new List<ProjectDetail>();
            foreach (var id in ids)
            {
                try
                {
                    var project = await GetAsync(id);
                    if (project != null)
                        projects.Add(project);
                }
                catch
                {
                    // Skip projects that fail to load
                }
            }
            return projects;
        }
    }

    #endregion

    #region Tasks Resource

    /// <summary>
    /// Resource client for task operations.
    /// </summary>
    public class TasksResource : BulkApiClientBase<
        TaskSummary,
        TaskDetail,
        CreateTaskRequest,
        UpdateTaskRequest,
        TaskQueryParams,
        BulkCreateTasksRequest,
        BulkUpdateTasksRequest,
        BulkDeleteTasksRequest>
    {
        private readonly ProjectManagementApi _module;

        // Use legacy path for backwards compatibility (api/tasks)
        protected override string BasePath => "api/tasks";

        internal TasksResource(IHttpClient http, ProjectManagementApi module) : base(http)
        {
            _module = module;
        }

        protected override IEnumerable<string> GetCustomQueryParams(TaskQueryParams query)
        {
            var @params = new List<string>();

            if (query.ProjectId.HasValue)
                @params.Add($"projectId={query.ProjectId.Value}");
            if (query.TaskTypeId.HasValue)
                @params.Add($"taskTypeId={query.TaskTypeId.Value}");
            if (query.TaskStatusId.HasValue)
                @params.Add($"taskStatusId={query.TaskStatusId.Value}");
            if (query.AssignedToId.HasValue)
                @params.Add($"assignedToId={query.AssignedToId.Value}");
            if (query.IsOpen.HasValue)
                @params.Add($"isOpen={query.IsOpen.Value}");

            return @params;
        }

        protected override List<TaskSummary>? MapToEntityList(List<TaskDetail>? details)
        {
            return details?.Select(d => new TaskSummary
            {
                Id = d.Id,
                Name = d.Name,
                TaskNo = d.TaskNo,
                ProjectId = d.ProjectId,
                ProjectName = d.ProjectName,
                TaskStatusId = d.TaskStatusId,
                TaskTypeId = d.TaskTypeId,
                AssignedToId = d.AssignedToId,
                AssignedToName = d.AssignedToName,
                DueDate = d.DueDate,
                IsOpen = d.IsOpen
            }).ToList();
        }

        /// <summary>
        /// Gets tasks for a specific project.
        /// </summary>
        public Task<List<TaskSummary>> GetByProjectAsync(int projectId, int take = 100)
        {
            return GetAllAsync(new TaskQueryParams { ProjectId = projectId, Take = take });
        }

        /// <summary>
        /// Gets tasks assigned to a specific user.
        /// </summary>
        public Task<List<TaskSummary>> GetByAssigneeAsync(int userId, bool openOnly = true, int take = 100)
        {
            return GetAllAsync(new TaskQueryParams
            {
                AssignedToId = userId,
                IsOpen = openOnly ? true : null,
                Take = take
            });
        }

        /// <summary>
        /// Searches tasks by term.
        /// </summary>
        public Task<List<TaskSummary>> SearchAsync(string searchTerm, int take = 20)
        {
            return GetAllAsync(new TaskQueryParams
            {
                Search = searchTerm,
                Take = take,
                IsOpen = true
            });
        }

        /// <summary>
        /// Gets multiple tasks by IDs.
        /// </summary>
        public async Task<List<TaskDetail>> GetRangeAsync(List<int> ids)
        {
            var tasks = new List<TaskDetail>();
            foreach (var id in ids)
            {
                try
                {
                    var task = await GetAsync(id);
                    if (task != null)
                        tasks.Add(task);
                }
                catch
                {
                    // Skip tasks that fail to load
                }
            }
            return tasks;
        }
    }

    #endregion
}
