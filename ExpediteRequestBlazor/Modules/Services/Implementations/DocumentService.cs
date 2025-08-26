using ExpediteRequestBlazor.DataTransferObjects;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace ExpediteRequestBlazor.Modules.Services.Implementations
{
    public class DocumentService : IDocumentService
    {
        private readonly IDocumentRepository _documentRepository;
        private readonly IApprovalRepository _approvalRepository;
        private readonly IDbContextFactory<ExpediteRequestContext> _contextFactory;
        public readonly ILogger<DocumentRepository> _logger;
        private readonly IConfiguration _config;
        private readonly User _user;

        public DocumentService(IDbContextFactory<ExpediteRequestContext> contextFactory, ILogger<DocumentRepository> logger, IConfiguration config, User user, IDocumentRepository documentRepository, IApprovalRepository approvalRepository)
        {
            _contextFactory = contextFactory;
            _logger = logger;
            _config = config;
            _user = user;
            _documentRepository = documentRepository;
            _approvalRepository = approvalRepository;
        }

        public async Task SaveObject(Document document)
        {
            await using ExpediteRequestContext context = await _contextFactory.CreateDbContextAsync();

            if (document.Gkey == Guid.Empty)
            {
                document.Status = Status.SUBMITTED;
                document.CreatedBy = _user.Employee.SamaccountName;
                document.Modified = DateTime.UtcNow;
                document.ModifiedBy = _user.Employee.SamaccountName;

                await _documentRepository.Create(context, document);
            }
            else
            {
                ValidateDocumentExists(document);

                document.Modified = DateTime.UtcNow;
                document.ModifiedBy = _user.Employee.SamaccountName;

                Approval approval = new();
                if (document.Approvals == null || document.Approvals.Count == 0)
                {
                    approval = new()
                    {
                        Title = "Manager",
                        Signature = _user.Employee.FullName,
                        Date = DateTime.UtcNow,
                        Remarks = document.ApproverComments,
                        DocumentId = document.Id,
                        ApprovalType = "Approve",
                        ApprovalStatus = "Approve Total"
                    };
                    await _approvalRepository.Create(context, approval);
                }
                else
                {
                    await _approvalRepository.Update(context, approval);
                }
                await _documentRepository.Update(context, document);
            }
        }

        public void ValidateDocumentExists(Document item)
        {
            using ExpediteRequestContext context = _contextFactory.CreateDbContext();
            var result = _documentRepository.DocumentExists(context, item);
            if (!result)
            {
                throw new ArgumentException("Document does not exist", nameof(item));
            }
        }

        public async Task<Document> Get(Guid Gkey)
        {
            await using ExpediteRequestContext context = await _contextFactory.CreateDbContextAsync();
            return await _documentRepository.Get(context, Gkey);
        }

        public async Task HandleObject(Document document)
        {
            await SaveObject(document);
            string OriganatorEmail = "";
            if (!string.IsNullOrWhiteSpace(document.CreatedBy))
            {
                OriganatorEmail = document.CreatedBy + "@indium.com";
            }

            ElsaDocument elsaDocument = new(document)
            {
                EmailAPI = _config["APIs:EmailAPI"] ?? "",
                OrigninatorEmail = OriganatorEmail,
                AppLocation = _config["AppLocation"] ?? "",
            };
            await UpdateObjectStatus(document, elsaDocument);
        }

        public async Task UpdateObjectStatus(Document document, ElsaDocument elsaDocument)
        {
            HttpContent content;
            Document contentObject;

            content = await GetWorkflowEngineResponseAsync(elsaDocument);
            contentObject = await ParseWorkflowEngineResponseAsync<Document>(content);
            await using ExpediteRequestContext context = await _contextFactory.CreateDbContextAsync();
            document = contentObject;

            await _documentRepository.Update(context, document);
        }

        public async Task<HttpContent> GetWorkflowEngineResponseAsync<T>(T item)
        {
            HttpClient client = new(new HttpClientHandler() { UseDefaultCredentials = true });
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            StringContent content = new(
                System.Text.Json.JsonSerializer.Serialize(item),
                System.Text.Encoding.UTF8,
                "application/json"
            );
            HttpResponseMessage response = await client.PostAsync(_config["APIs:WorkflowEngine"] + "/ExpediteRequest/ProcessDocument", content);
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Invalid response from workflow engine");
            }
            return response.Content;
        }

        public async Task<T> ParseWorkflowEngineResponseAsync<T>(HttpContent content)
        {
            string stringContent = await content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<T>(stringContent);
        }

        public IQueryable<ExpediteRequestsExtended> GetAll()
        {
            ExpediteRequestContext context = _contextFactory.CreateDbContext();
            return _documentRepository.GetAll(context);
        }

        public IQueryable<ExpediteRequestsExtended> GetAll_ProductionPlanner()
        {
            ExpediteRequestContext context = _contextFactory.CreateDbContext();
            return _documentRepository.GetAll_ProductionPlanner(context);
        }
    }
}
