using System.IO;
using System.Xml;
using System.Xml.Schema;

namespace XML_FAB.Services;

public sealed record XmlValidationResult(bool IsValid, IReadOnlyList<string> Messages);

public sealed class XmlSchemaValidationService
{
    public XmlValidationResult Validate(string xmlPath, string schemaPath)
    {
        var messages = new List<string>();
        var schemaIsValid = true;
        var documentHasErrors = false;

        try
        {
            var schemas = new XmlSchemaSet { XmlResolver = null };
            schemas.ValidationEventHandler += (_, args) =>
            {
                messages.Add(Format(args));
                if (args.Severity == XmlSeverityType.Error)
                    schemaIsValid = false;
            };
            schemas.Add(null, schemaPath);
            schemas.Compile();

            if (!schemaIsValid)
                return new XmlValidationResult(false, messages);

            var settings = new XmlReaderSettings
            {
                ValidationType = ValidationType.Schema,
                Schemas = schemas,
                DtdProcessing = DtdProcessing.Prohibit,
                XmlResolver = null
            };
            settings.ValidationFlags |= XmlSchemaValidationFlags.ProcessIdentityConstraints;
            settings.ValidationFlags |= XmlSchemaValidationFlags.ReportValidationWarnings;
            settings.ValidationEventHandler += (_, args) =>
            {
                messages.Add(Format(args));
                if (args.Severity == XmlSeverityType.Error)
                    documentHasErrors = true;
            };

            using var reader = XmlReader.Create(xmlPath, settings);
            while (reader.Read()) { }

            return new XmlValidationResult(!documentHasErrors, messages);
        }
        catch (XmlException ex)
        {
            messages.Add($"Ошибка XML (строка {ex.LineNumber}, позиция {ex.LinePosition}): {ex.Message}");
        }
        catch (XmlSchemaException ex)
        {
            messages.Add($"Ошибка XSD (строка {ex.LineNumber}, позиция {ex.LinePosition}): {ex.Message}");
        }
        catch (IOException ex)
        {
            messages.Add($"Ошибка чтения файла: {ex.Message}");
        }
        catch (UnauthorizedAccessException ex)
        {
            messages.Add($"Нет доступа к файлу: {ex.Message}");
        }

        return new XmlValidationResult(false, messages);
    }

    private static string Format(ValidationEventArgs args)
    {
        var prefix = args.Severity == XmlSeverityType.Error ? "Ошибка" : "Предупреждение";
        var line = args.Exception is { LineNumber: > 0 } ex
            ? $" (строка {ex.LineNumber}, позиция {ex.LinePosition})"
            : string.Empty;
        return $"{prefix}{line}: {args.Message}";
    }
}
