CREATE OR ALTER VIEW [dbo].[ExpediteRequestsExtended]
AS
SELECT  d.*,
		a.[Signature] as Approver,
		a.[ApprovalStatus] as ApprovalStatus,
		a.[Remarks] as Remarks
  FROM [ExpediteRequest].[dbo].[Document] as d
  LEFT JOIN dbo.Approval as a
  ON a.DocumentID = d.Id
