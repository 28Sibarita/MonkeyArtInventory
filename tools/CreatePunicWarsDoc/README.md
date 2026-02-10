# CreatePunicWarsDoc

Herramienta de consola en C# para generar un documento de Microsoft Word sobre las Guerras Púnicas.

## Uso / Usage

```bash
cd tools/CreatePunicWarsDoc
dotnet run
```

El documento se generará en `docs/Guerras_Punicas.docx`.

O especifica una ruta personalizada:

```bash
dotnet run /ruta/personalizada/documento.docx
```

## Requisitos / Requirements

- .NET 10.0 SDK
- DocumentFormat.OpenXml library (incluido en el proyecto)

## Descripción / Description

Esta herramienta genera un documento Word (.docx) completamente formateado que contiene información histórica detallada sobre las tres Guerras Púnicas entre Roma y Cartago (264-146 a.C.).

El documento incluye:
- Títulos y encabezados estructurados
- Listas con viñetas
- Contenido histórico en español

---

This tool generates a fully formatted Word document (.docx) containing detailed historical information about the three Punic Wars between Rome and Carthage (264-146 BC).

The document includes:
- Structured titles and headings
- Bulleted lists
- Historical content in Spanish
