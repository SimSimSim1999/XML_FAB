using System.Xml.Linq;

namespace XML_FAB.Models;

public sealed class ExplanatoryNoteModel
{
    public string SchemaVersion { get; set; } = "01.07";
    public string ExplanatoryNoteNumber { get; set; } = "";
    public string ExplanatoryNoteYear { get; set; } = DateTime.Now.Year.ToString();
    public string AccessRestriction { get; set; } = "";
    public string Id { get; set; } = Guid.NewGuid().ToString().ToUpperInvariant();

    public List<ModificationItem> ExplanatoryNoteModifications { get; set; } = new();
    public OrganizationInfo IssueAuthor { get; set; } = new();
    public List<SignerInfo> Signers { get; set; } = new();
    public OrganizationInfo Developer { get; set; } = new();
    public List<string> UsedNorms { get; set; } = new();
    public List<ProjectDocument> ProjectDecisionDocuments { get; set; } = new();
    public List<ProjectDocument> ProjectInitialDocuments { get; set; } = new();
    public List<AdditionalDataItem> AdditionalData { get; set; } = new();
    public FinanceSourcesInfo FinanceSources { get; set; } = new();
    public ObjectInfo NonIndustrialObject { get; set; } = new();
    public List<DesignerNoteItem> DesignerNotes { get; set; } = new();
    public List<string> DesignerAssurance { get; set; } = new();

    public ExplanatoryNoteModel()
    {
        ExplanatoryNoteModifications.Add(new ModificationItem());
        IssueAuthor = new OrganizationInfo
        {
            FullName = "",
            AbbreviatedName = "",
            Ogrn = "",
            Inn = "",
            Kpp = "",
            NOPRIZNumber = "",
            Address = new AddressInfo()
        };

        Developer = new OrganizationInfo
        {
            FullName = "",
            AbbreviatedName = "",
            Ogrn = "",
            Inn = "",
            Kpp = "",
            NOPRIZNumber = "",
            Address = new AddressInfo()
        };

        Signers.Add(new SignerInfo());
        UsedNorms.Add("Нормативные документы не указаны");
        ProjectDecisionDocuments.Add(new ProjectDocument());
        ProjectInitialDocuments.Add(new ProjectDocument());
        AdditionalData.Add(new AdditionalDataItem { DataDescription = "", File = new FileInfo { FileName = "", FileFormat = "pdf", FileChecksum = "" } });
        NonIndustrialObject = new ObjectInfo
        {
            TypeName = "NonIndustrialObject",
            ObjectId = Guid.NewGuid().ToString().ToUpperInvariant(),
            Name = "",
            ConstructionType = "1",
            Address = new AddressInfo()
        };
        DesignerNotes.Add(new DesignerNoteItem { Title = "", Text = new List<string> { "" } });
        DesignerAssurance.Add("");
    }

    public XElement ToXml()
    {
        XNamespace xsi = "http://www.w3.org/2001/XMLSchema-instance";

        var root = new XElement(
            "ExplanatoryNote",
            new XAttribute("SchemaVersion", SchemaVersion),
            new XAttribute("id", Id),
            new XAttribute(XNamespace.Xmlns + "xsi", xsi.NamespaceName),
            new XAttribute(xsi + "noNamespaceSchemaLocation", "explanatorynote-01-07.xsd"));

        if (!string.IsNullOrWhiteSpace(AccessRestriction))
            root.Add(new XAttribute("AccessRestriction", AccessRestriction));

        root.Add(new XElement("ExplanatoryNoteNumber", ExplanatoryNoteNumber));
        root.Add(new XElement("ExplanatoryNoteYear", ExplanatoryNoteYear));

        if (ExplanatoryNoteModifications.Count > 0)
        {
            var modifications = new XElement("ExplanatoryNoteModifications");
            foreach (var item in ExplanatoryNoteModifications)
                modifications.Add(item.ToXml());
            root.Add(modifications);
        }

        root.Add(IssueAuthor.ToXml("IssueAuthor"));

        if (Signers.Count > 0)
        {
            var signers = new XElement("Signers");
            foreach (var signer in Signers)
                signers.Add(signer.ToXml());
            root.Add(signers);
        }

        root.Add(Developer.ToXml("Developer"));

        if (UsedNorms.Count > 0)
        {
            var usedNorms = new XElement("UsedNorms");
            foreach (var text in UsedNorms)
                usedNorms.Add(new XElement("UsedNorm", text));
            root.Add(usedNorms);
        }

        if (ProjectDecisionDocuments.Count > 0)
        {
            var documents = new XElement("ProjectDecisionDocuments");
            foreach (var document in ProjectDecisionDocuments)
                documents.Add(document.ToXml());
            root.Add(documents);
        }

        if (ProjectInitialDocuments.Count > 0)
        {
            var documents = new XElement("ProjectInitialDocuments");
            foreach (var document in ProjectInitialDocuments)
                documents.Add(document.ToXml());
            root.Add(documents);
        }

        if (AdditionalData.Count > 0)
        {
            var additional = new XElement("AdditionalData");
            foreach (var item in AdditionalData)
                additional.Add(item.ToXml());
            root.Add(additional);
        }

        root.Add(FinanceSources.ToXml());
        root.Add(NonIndustrialObject.ToXml());

        foreach (var note in DesignerNotes)
            root.Add(note.ToXml());

        if (DesignerAssurance.Count > 0)
        {
            var assurance = new XElement("DesignerAssurance");
            foreach (var text in DesignerAssurance)
                assurance.Add(new XElement("Text", text));
            root.Add(assurance);
        }

        return root;
    }
}

public sealed class ModificationItem
{
    public string ModificationNumber { get; set; } = "1";
    public string ModificationDate { get; set; } = DateTime.Today.ToString("yyyy-MM-dd");
    public string ModificationNote { get; set; } = "";

    public XElement ToXml()
    {
        return new XElement("Modification",
            new XElement("ModificationNumber", ModificationNumber),
            new XElement("ModificationDate", ModificationDate),
            new XElement("ModificationNote", ModificationNote));
    }
}

public sealed class AddressInfo
{
    public string RegionCode { get; set; } = "02";
    public string OktmoCode { get; set; } = "80701000001";
    public string OktmoName { get; set; } = "г. Уфа";
    public string District { get; set; } = "";
    public string City { get; set; } = "Уфа";
    public string Street { get; set; } = "Центральная";
    public string Building { get; set; } = "1";
    public string Room { get; set; } = "";

    public XElement ToXml(string elementName = "Address")
    {
        var address = new XElement(elementName,
            new XElement("RussianAddress",
                new XElement("RegionCode", RegionCode),
                new XElement("OKTMOCode", OktmoCode),
                new XElement("OKTMOName", OktmoName)));

        if (!string.IsNullOrWhiteSpace(District))
            address.Element("RussianAddress")?.Add(new XElement("District", District));

        address.Element("RussianAddress")?.Add(
            new XElement("City", City),
            new XElement("Street", Street),
            new XElement("Building", Building));

        if (!string.IsNullOrWhiteSpace(Room))
            address.Element("RussianAddress")?.Add(new XElement("Room", Room));

        return address;
    }
}

public sealed class OrganizationInfo
{
    public string FullName { get; set; } = "";
    public string AbbreviatedName { get; set; } = "";
    public string Ogrn { get; set; } = "";
    public string Inn { get; set; } = "";
    public string Kpp { get; set; } = "";
    public string NOPRIZNumber { get; set; } = "";
    public AddressInfo Address { get; set; } = new();

    public XElement ToXml(string elementName)
    {
        var organization = new XElement("Organization",
            new XElement("FullName", FullName),
            new XElement("AbbreviatedName", AbbreviatedName),
            new XElement("OGRN", Ogrn),
            new XElement("INN", Inn));

        if (!string.IsNullOrWhiteSpace(Kpp))
            organization.Add(new XElement("KPP", Kpp));

        if (!string.IsNullOrWhiteSpace(NOPRIZNumber) && elementName == "Organization")
            organization.Add(new XAttribute("NOPRIZNumber", NOPRIZNumber));

        if (Address != null)
            organization.Add(Address.ToXml());

        return new XElement(elementName, organization);
    }
}

public sealed class SignerInfo
{
    public string Role { get; set; } = "Разработка";
    public string Surname { get; set; } = "";
    public string Name { get; set; } = "";
    public string Patronymic { get; set; } = "";
    public string Snils { get; set; } = "";
    public string NOPRIZNumber { get; set; } = "";

    public XElement ToXml()
    {
        return new XElement("ChiefProjectEngineer",
            new XAttribute("role", Role),
            new XElement("Surname", Surname),
            new XElement("Name", Name),
            new XElement("Patronymic", Patronymic),
            new XElement("SNILS", Snils),
            new XElement("NOPRIZNumber", NOPRIZNumber));
    }
}

public sealed class FileInfo
{
    public string FileName { get; set; } = "";
    public string FileFormat { get; set; } = "pdf";
    public string FileChecksum { get; set; } = "";
    public SignFileInfo? SignFile { get; set; }

    public XElement ToXml()
    {
        var file = new XElement("File",
            new XElement("FileName", FileName),
            new XElement("FileFormat", FileFormat),
            new XElement("FileChecksum", FileChecksum));

        if (SignFile != null)
            file.Add(SignFile.ToXml());

        return file;
    }
}

public sealed class SignFileInfo
{
    public string FileName { get; set; } = "";
    public string FileFormat { get; set; } = "sig";
    public string RelativePath { get; set; } = "ЭЦП";
    public string FileChecksum { get; set; } = "";

    public XElement ToXml()
    {
        return new XElement("SignFile",
            new XElement("FileName", FileName),
            new XElement("FileFormat", FileFormat),
            new XElement("RelativePath", RelativePath),
            new XElement("FileChecksum", FileChecksum));
    }
}

public sealed class ProjectDocument
{
    public string DocType { get; set; } = "";
    public string DocName { get; set; } = "";
    public string DocNumber { get; set; } = "";
    public string DocDate { get; set; } = "";
    public string DocIssueAuthor { get; set; } = "";
    public string Included { get; set; } = "";
    public FileInfo? File { get; set; }

    public XElement ToXml()
    {
        var document = new XElement("Document",
            new XElement("DocType", DocType),
            new XElement("DocName", DocName),
            new XElement("DocNumber", DocNumber),
            new XElement("DocDate", DocDate),
            new XElement("DocIssueAuthor", DocIssueAuthor));

        if (!string.IsNullOrWhiteSpace(Included))
            document.Add(new XElement("Included", Included));

        if (File != null)
            document.Add(File.ToXml());

        return document;
    }
}

public sealed class AdditionalDataItem
{
    public string DataDescription { get; set; } = "";
    public FileInfo File { get; set; } = new();

    public XElement ToXml()
    {
        return new XElement("Data",
            new XElement("DataDescription", DataDescription),
            File.ToXml());
    }
}

public sealed class FinanceSourcesInfo
{
    public bool PrivateSource { get; set; } = true;
    public string FinanceComment { get; set; } = "";

    public XElement ToXml()
    {
        var node = new XElement("FinanceSources");
        if (PrivateSource)
            node.Add(new XElement("PrivateSource"));
        if (!string.IsNullOrWhiteSpace(FinanceComment))
            node.Add(new XElement("FinanceComment", FinanceComment));
        return node;
    }
}

public sealed class ObjectInfo
{
    public string TypeName { get; set; } = "NonIndustrialObject";
    public string ObjectId { get; set; } = "";
    public string Placement { get; set; } = "1";
    public string DangerousAndComplex { get; set; } = "0";
    public string Unique { get; set; } = "0";
    public string Name { get; set; } = "";
    public string ConstructionType { get; set; } = "1";
    public AddressInfo Address { get; set; } = new();

    public XElement ToXml()
    {
        return new XElement(TypeName,
            new XAttribute("ObjectID", ObjectId),
            new XAttribute("Placement", Placement),
            new XAttribute("DangerousAndComplex", DangerousAndComplex),
            new XAttribute("Unique", Unique),
            new XElement("Name", Name),
            new XElement("ConstructionType", ConstructionType),
            Address.ToXml());
    }
}

public sealed class DesignerNoteItem
{
    public string Title { get; set; } = "";
    public List<string> Text { get; set; } = new();

    public XElement ToXml()
    {
        var note = new XElement("DesignerNote",
            new XAttribute("Title", Title));

        foreach (var item in Text)
            note.Add(new XElement("Text", item));

        return note;
    }
}

public sealed class TeiItem
{
    public string Name { get; set; } = "";
    public string Measure { get; set; } = "";
    public string Value { get; set; } = "";

    public XElement ToXml()
    {
        return new XElement("TEI",
            new XElement("Name", Name),
            new XElement("Measure", Measure),
            new XElement("Value", Value));
    }
}
