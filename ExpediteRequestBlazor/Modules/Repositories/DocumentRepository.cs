using Indium.Common.DataTransferObjects;
using Indium.Common.Models;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using ExpediteRequestBlazor.DataTransferObjects;
using System.Data;


namespace ExpediteRequestBlazor.Repositories
{
    public class DocumentRepository : IDocumentRepository
    {
        private readonly IDbContextFactory<ExpediteRequestContext> _contextFactory;
        public readonly ILogger<DocumentRepository> _logger;
        private readonly IConfiguration _config;
        private readonly User _user;

        public DocumentRepository(IDbContextFactory<ExpediteRequestContext> contextFactory, ILogger<DocumentRepository> logger, IConfiguration config, User user)
        {
            _contextFactory = contextFactory;
            _logger = logger;
            _config = config;
            _user = user;
        }

        public async Task SaveObject(Document document)
        {
            await using ExpediteRequestContext context = await _contextFactory.CreateDbContextAsync();

            if (document.Gkey == Guid.Empty) // New Object (create)
            {
                document.Status = Status.SUBMITTED;
                document.CreatedBy = _user.Employee.SamaccountName;
                document.Modified = DateTime.UtcNow;
                document.ModifiedBy = _user.Employee.SamaccountName;

                await context.Documents.AddAsync(document);
                await context.SaveChangesAsync();
            }
            else // Existing object (update)
            {
                ValidateDocumentExists(document);

                document.Modified = DateTime.UtcNow;
                document.ModifiedBy = _user.Employee.SamaccountName;

                Approval approval = new();
                if (document.Approvals == null)
                {
                    approval = new()
                    {
                        Title = "Manager",
                        Signature = _user.Employee.FullName,
                        Date = DateTime.UtcNow,
                        Remarks = document.Comments,
                        DocumentId = document.Id,
                        ApprovalType = "Approve",
                        ApprovalStatus = "Approve Total"
                    };
                    context.Approvals.Add(approval);
                }
                else
                {
                    context.Approvals.Update(approval);   
                }
                context.Documents.Update(document);
                await context.SaveChangesAsync();
            }
        }

        public bool DocumentExists(Document item)
        {
            using ExpediteRequestContext context = _contextFactory.CreateDbContext();
            return context.Documents.Any(doc => doc.Gkey == item.Gkey);
        }

        public void ValidateDocumentExists(Document item)
        {
            if (!DocumentExists(item))
            {
                throw new ArgumentException("Document does not exist", nameof(item));
            }
        }

        public bool DocumentExists(DocumentModel item)
        {
            using ExpediteRequestContext context = _contextFactory.CreateDbContext();
            return context.Documents.Any(doc => doc.Gkey == item.Gkey);
        }

        public async Task HandleObject(Document document)
        {
            await SaveObject(document);
            string OriganatorEmail = "";
            if (!string.IsNullOrWhiteSpace(document.CreatedBy) && document.CreatedBy.Contains("\\")) {
                string[] OriganatorEmailList = document.CreatedBy.Split("\\");
                OriganatorEmail += OriganatorEmailList[1];
                OriganatorEmail += "@indium.com";
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

            context.Documents.Update(document);
            await context.SaveChangesAsync();
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
            String stringContent = await content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<T>(stringContent);
        }

        public async Task<HttpContent> CreateActionItem(Document item)
        {
            ActionItem action = new();
            action.ApplicationId = 11;
            action.AssignedTo = "PUT EMAIL HERE";
            action.CreatedBy = "system";
            action.CreatedDate = System.DateTime.UtcNow;
            action.EditedBy = "";
            action.Title = "";
            action.URL = "https://apps-dev.ica.com/RemoteWorkRequest/NewDocument/" + item.Gkey;


            HttpClient client = new(new HttpClientHandler() { UseDefaultCredentials = true });
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            StringContent content = new(
                System.Text.Json.JsonSerializer.Serialize(action),
                System.Text.Encoding.UTF8,
                "application/json"
            );

            HttpResponseMessage response = await client.PostAsync(_config["APIs:ActionItems"], content);
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Invalid response from workflow engine");
            }
            return response.Content;
        }

        public async Task<Document> RetrieveObject(Guid Gkey)
        {
            try
            {
                await using ExpediteRequestContext context = await _contextFactory.CreateDbContextAsync();
                IQueryable<Document> documentQuery = context.Documents.AsQueryable();
                IQueryable<Approval> ApproverQuery = context.Approvals.AsQueryable();
                Document document = await documentQuery.Where(doc => doc.Gkey == Gkey).FirstOrDefaultAsync();
                document.Approvals = ApproverQuery.Where(b => b.DocumentId == document.Id).ToList();

                return document;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.StackTrace);
                return null;
            }
        }


        public IQueryable<ExpediteRequestsExtended> GetDocuments()
        {
            ExpediteRequestContext context = _contextFactory.CreateDbContext();
            var result = context.ExpediteRequestsExtended
                .AsNoTracking()
                .Where(d => d.Status != Status.SUBMITTED)
                .AsQueryable();

            return result.AsQueryable();
        }

        public IQueryable<ExpediteRequestsExtended> GetProductionPlannerDocuments()
        {
            ExpediteRequestContext context = _contextFactory.CreateDbContext();
            var result = context.ExpediteRequestsExtended
                .AsNoTracking()
                .Where(d => d.Status == Status.AWAITING_PRODUCTION)
                .AsQueryable();

            return result.AsQueryable();
        }
    }
}
