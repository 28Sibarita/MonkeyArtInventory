using System.ComponentModel.DataAnnotations;

namespace MonkeyArtInventory.Core.Models;

public enum ProductType
{
    [Display(Name = "Muneco 3D")]
    Muneco3D = 0,

    [Display(Name = "Cuadro")]
    Cuadro = 1
}

public enum MovementType
{
    [Display(Name = "Entrada")]
    Entrada = 0,

    [Display(Name = "Salida")]
    Salida = 1,

    [Display(Name = "Ajuste")]
    Ajuste = 2,

    [Display(Name = "Merma")]
    Merma = 3
}
