using ExpediteRequestBlazor.DataTransferObjects;
using Indium.Common.DataTransferObjects;
using Indium.Common.Modules;
using Newtonsoft.Json;
using System.Linq.Expressions;
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
                await ValidateDocumentExists(document);

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
                        ApprovalStatus = document.ApprovalStatus
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

        public async Task ValidateDocumentExists(Document item)
        {
            using ExpediteRequestContext context = await _contextFactory.CreateDbContextAsync();
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

        public async Task<List<ExpediteRequestsExtended>> GetAll_Export(DateRange? dateRange = null)
        {
            List<ExpediteRequestsExtended> result = new();
            using ExpediteRequestContext context = await _contextFactory.CreateDbContextAsync();
            if (dateRange is not null)
            {
                result = await _documentRepository.GetBy_RequestExtended(context, (x => x.Created >= dateRange.Start && x.Created <= dateRange.End));
            }
            else
            {
                result = await GetAll().ToListAsync();
            }
            return result;
        }

        public IQueryable<ExpediteRequestsExtended> GetAll_ProductionPlanner()
        {
            ExpediteRequestContext context = _contextFactory.CreateDbContext();
            return _documentRepository.GetAll_ProductionPlanner(context);
        }

        public async Task<List<ExpediteRequestsExtended>> GetBy_RequestExtended(Expression<Func<ExpediteRequestsExtended, bool>> selector)
        {
            using ExpediteRequestContext context = await _contextFactory.CreateDbContextAsync();

            return await _documentRepository.GetBy_RequestExtended(context, selector);
        }

        public async Task DownloadExcelFile(List<ExpediteRequestsExtended> data, IJSRuntime jsRuntime, ILogger logger)
        {

            FlatExcelOptions<ExpediteRequestsExtended> excelOptions = new();
            excelOptions.Items = data;
            excelOptions.AddHeader(nameof(ExpediteRequestsExtended.Status), "Expedite Status");
            excelOptions.AddHeader(nameof(ExpediteRequestsExtended.InProgress), "In Progress");
            excelOptions.AddHeader(nameof(ExpediteRequestsExtended.CreatedBy), "Created By");
            excelOptions.AddHeader(nameof(ExpediteRequestsExtended.CoNum), "Order Number");
            excelOptions.AddHeader(nameof(ExpediteRequestsExtended.CoLine), "Line");
            excelOptions.AddHeader(nameof(ExpediteRequestsExtended.CoRelease), "Release");
            excelOptions.AddHeader(nameof(ExpediteRequestsExtended.PlanCode), "Planner Code");
            excelOptions.AddHeader(nameof(ExpediteRequestsExtended.ShipSite), "Ship Site");
            excelOptions.AddHeader(nameof(ExpediteRequestsExtended.Job), "Job Number");
            excelOptions.AddHeader(nameof(ExpediteRequestsExtended.Ipn), "IPN");
            excelOptions.AddHeader(nameof(ExpediteRequestsExtended.IpnDescription), "IPN Description");
            excelOptions.AddHeader(nameof(ExpediteRequestsExtended.OrderDate), "Order Date");
            excelOptions.AddHeader(nameof(ExpediteRequestsExtended.CustomerName), "Customer Name");
            excelOptions.AddHeader(nameof(ExpediteRequestsExtended.QtyOrdered), "Quantity");
            excelOptions.AddHeader(nameof(ExpediteRequestsExtended.Um), "UOM");
            excelOptions.AddHeader(nameof(ExpediteRequestsExtended.DueDate), "Due Date");
            excelOptions.AddHeader(nameof(ExpediteRequestsExtended.ShelfLifeRequirement), "Shelf Life Required");
            excelOptions.AddHeader(nameof(ExpediteRequestsExtended.PonderosaPack), "Ponderosa Pack");
            excelOptions.AddHeader(nameof(ExpediteRequestsExtended.CurrentDueDate), "Current Due Date");
            excelOptions.AddHeader(nameof(ExpediteRequestsExtended.PartialQuantityAccepted), "Partial Accepted");
            excelOptions.AddHeader(nameof(ExpediteRequestsExtended.PartialQuantity), "Partial Quantity");
            excelOptions.AddHeader(nameof(ExpediteRequestsExtended.ShipDate), "Request Ship Date");
            excelOptions.AddHeader(nameof(ExpediteRequestsExtended.Fee), "Expedite Fee");
            excelOptions.AddHeader(nameof(ExpediteRequestsExtended.Reason), "Reason for Expedite");
            excelOptions.AddHeader(nameof(ExpediteRequestsExtended.Comments), "Comments");
            excelOptions.AddHeader(nameof(ExpediteRequestsExtended.Approver), "Approver");
            excelOptions.AddHeader(nameof(ExpediteRequestsExtended.ApprovalStatus), "Approve Status");
            excelOptions.AddHeader(nameof(ExpediteRequestsExtended.ApprovalRemarks), "Approver Comments");
            excelOptions.AddHeader(nameof(ExpediteRequestsExtended.NewShipDate), "New Ship Date");
            excelOptions.AddHeader(nameof(ExpediteRequestsExtended.Created), "Created Date");

            try
            {
                NpoiMemoryStream stream = await CreateFlatExcel.StreamExcelFile<ExpediteRequestsExtended>(excelOptions, logger);
                using var FileStream = new DotNetStreamReference(stream);
                string fileName = "ExpediteRequest.xlsx";
                await jsRuntime.InvokeVoidAsync("downloadFileFromStream", fileName, FileStream);
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
            }
        }

        public async Task<List<string>>GetAll_SitesFromRequests()
        {
            using ExpediteRequestContext context = await _contextFactory.CreateDbContextAsync();
            return await _documentRepository.GetAll_SitesFromRequests(context);
        }
    }
}
