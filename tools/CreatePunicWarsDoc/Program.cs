using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;

class Program
{
    static void Main(string[] args)
    {
        string outputPath = args.Length > 0 
            ? args[0] 
            : Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "docs", "Guerras_Punicas.docx");

        // Ensure the directory exists
        var directory = Path.GetDirectoryName(outputPath);
        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        Console.WriteLine($"Creating Word document about the Punic Wars at: {outputPath}");

        CreateWordDocument(outputPath);

        Console.WriteLine("Document created successfully!");
    }

    static void CreateWordDocument(string filepath)
    {
        // Create a new Word document
        using (WordprocessingDocument wordDocument = WordprocessingDocument.Create(filepath, WordprocessingDocumentType.Document))
        {
            // Add a main document part
            MainDocumentPart mainPart = wordDocument.AddMainDocumentPart();

            // Create the document structure
            mainPart.Document = new Document();
            Body body = mainPart.Document.AppendChild(new Body());

            // Title
            AddHeading(body, "Las Guerras Púnicas", "Heading1");
            AddParagraph(body, "");

            // Introduction
            AddHeading(body, "Introducción", "Heading2");
            AddParagraph(body, "Las Guerras Púnicas fueron una serie de tres conflictos bélicos que enfrentaron a las dos grandes potencias del Mediterráneo occidental: Roma y Cartago. Estos conflictos se desarrollaron entre los años 264 a.C. y 146 a.C., y determinaron el destino del mundo antiguo, consolidando a Roma como la potencia dominante del Mediterráneo.");
            AddParagraph(body, "");

            // First Punic War
            AddHeading(body, "Primera Guerra Púnica (264-241 a.C.)", "Heading2");
            AddParagraph(body, "La Primera Guerra Púnica se desarrolló principalmente en Sicilia y en el mar. Fue el primer conflicto a gran escala entre Roma y Cartago, desencadenado por disputas territoriales en Sicilia.");
            
            AddHeading(body, "Causas principales:", "Heading3");
            AddBulletList(body, new[]
            {
                "Conflicto por el control de Sicilia",
                "Rivalidad comercial en el Mediterráneo occidental",
                "Expansión territorial de ambas potencias"
            });

            AddHeading(body, "Resultado:", "Heading3");
            AddParagraph(body, "Roma salió victoriosa, obligando a Cartago a abandonar Sicilia y pagar una enorme indemnización de guerra. Roma se convirtió en una potencia naval por primera vez en su historia.");
            AddParagraph(body, "");

            // Second Punic War
            AddHeading(body, "Segunda Guerra Púnica (218-201 a.C.)", "Heading2");
            AddParagraph(body, "Esta es la más famosa de las Guerras Púnicas, principalmente por las hazañas militares de Aníbal Barca, quien cruzó los Alpes con su ejército, incluidos elefantes de guerra, para atacar Italia directamente.");

            AddHeading(body, "Eventos destacados:", "Heading3");
            AddBulletList(body, new[]
            {
                "Cruce de los Alpes por Aníbal (218 a.C.)",
                "Batalla del Lago Trasimeno (217 a.C.) - Victoria cartaginesa",
                "Batalla de Cannas (216 a.C.) - Mayor derrota romana, victoria de Aníbal",
                "Batalla de Zama (202 a.C.) - Victoria romana decisiva de Escipión el Africano"
            });

            AddHeading(body, "Resultado:", "Heading3");
            AddParagraph(body, "Roma derrotó a Cartago bajo el liderazgo de Publio Cornelio Escipión (Escipión el Africano). Cartago perdió todas sus posesiones fuera de África y quedó reducida a un estado cliente de Roma.");
            AddParagraph(body, "");

            // Third Punic War
            AddHeading(body, "Tercera Guerra Púnica (149-146 a.C.)", "Heading2");
            AddParagraph(body, "La última guerra fue esencialmente un sitio de la ciudad de Cartago, motivado por el deseo romano de eliminar completamente a su antigua rival.");

            AddHeading(body, "Desarrollo:", "Heading3");
            AddParagraph(body, "Roma, bajo la influencia de figuras como Catón el Viejo (quien terminaba todos sus discursos con 'Carthago delenda est' - 'Cartago debe ser destruida'), decidió acabar definitivamente con Cartago.");

            AddHeading(body, "Resultado:", "Heading3");
            AddParagraph(body, "Cartago fue completamente destruida en el 146 a.C. La ciudad fue arrasada, sus habitantes fueron vendidos como esclavos, y según la leyenda, la tierra fue sembrada con sal para que nada pudiera crecer allí. El territorio cartaginés se convirtió en la provincia romana de África.");
            AddParagraph(body, "");

            // Consecuencias
            AddHeading(body, "Consecuencias de las Guerras Púnicas", "Heading2");
            AddBulletList(body, new[]
            {
                "Roma se convirtió en la potencia dominante del Mediterráneo occidental",
                "Cartago fue completamente destruida como civilización independiente",
                "Roma adquirió vastos territorios: Sicilia, Cerdeña, Córcega, Hispania y el norte de África",
                "Se estableció el dominio romano que duraría siglos",
                "La economía romana se transformó con la afluencia de esclavos y riquezas",
                "Se sentaron las bases para el posterior Imperio Romano"
            });
            AddParagraph(body, "");

            // Personajes clave
            AddHeading(body, "Personajes Clave", "Heading2");
            
            AddHeading(body, "Por Cartago:", "Heading3");
            AddBulletList(body, new[]
            {
                "Aníbal Barca - El más grande general cartaginés",
                "Asdrúbal Barca - Hermano de Aníbal",
                "Amílcar Barca - Padre de Aníbal, comandante en la Primera Guerra Púnica"
            });

            AddHeading(body, "Por Roma:", "Heading3");
            AddBulletList(body, new[]
            {
                "Publio Cornelio Escipión el Africano - Derrotó a Aníbal en Zama",
                "Quinto Fabio Máximo - Desarrolló la estrategia de desgaste contra Aníbal",
                "Marco Claudio Marcelo - Defendió Sicilia",
                "Escipión Emiliano - Destruyó Cartago en la Tercera Guerra Púnica"
            });
            AddParagraph(body, "");

            // Conclusión
            AddHeading(body, "Conclusión", "Heading2");
            AddParagraph(body, "Las Guerras Púnicas fueron conflictos determinantes en la historia antigua. No solo decidieron quién dominaría el Mediterráneo, sino que también transformaron a Roma de una potencia regional italiana en un imperio que dominaría el mundo conocido durante siglos. La destrucción de Cartago marcó el fin de una de las civilizaciones más importantes del mundo antiguo y el inicio de la hegemonía romana indiscutible.");
            AddParagraph(body, "");
            AddParagraph(body, "El legado de estos conflictos se puede ver en numerosos aspectos de la cultura occidental, desde tácticas militares hasta estructuras políticas, y las historias de figuras como Aníbal continúan siendo estudiadas en academias militares de todo el mundo.");

            // Save the document
            mainPart.Document.Save();
        }
    }

    static void AddHeading(Body body, string text, string styleId)
    {
        Paragraph para = body.AppendChild(new Paragraph());
        Run run = para.AppendChild(new Run());
        run.AppendChild(new Text(text));

        ParagraphProperties paraProps = new ParagraphProperties();
        ParagraphStyleId paraStyle = new ParagraphStyleId() { Val = styleId };
        paraProps.AppendChild(paraStyle);
        para.PrependChild(paraProps);
    }

    static void AddParagraph(Body body, string text)
    {
        Paragraph para = body.AppendChild(new Paragraph());
        Run run = para.AppendChild(new Run());
        run.AppendChild(new Text(text));
    }

    static void AddBulletList(Body body, string[] items)
    {
        foreach (var item in items)
        {
            Paragraph para = body.AppendChild(new Paragraph());
            
            ParagraphProperties paraProps = new ParagraphProperties();
            NumberingProperties numProps = new NumberingProperties();
            NumberingLevelReference lvlRef = new NumberingLevelReference() { Val = 0 };
            NumberingId numId = new NumberingId() { Val = 1 };
            numProps.Append(lvlRef);
            numProps.Append(numId);
            paraProps.Append(numProps);
            para.Append(paraProps);

            Run run = para.AppendChild(new Run());
            run.AppendChild(new Text(item));
        }
    }
}
