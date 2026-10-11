# language: es
Característica: Eliminar productos
  Como usuario de la API
  Quiero eliminar los productos que ya no vendo
  Para mantener mi catálogo limpio

  Antecedentes:
    Dado que existe una categoría con nombre "Bebidas"
    Y que existen los productos:
      | nombre    | descripción      | categoría |
      | Coca-Cola | Refresco de cola | Bebidas   |
      | Sprite    | Refresco de lima | Bebidas   |

  Escenario: Eliminar un producto
    Cuando elimino el producto "Coca-Cola"
    Entonces el producto se elimina correctamente
    Y al consultar el detalle del producto eliminado no se encuentra

  Escenario: Eliminar un producto no afecta a los demás productos ni a su categoría
    Cuando elimino el producto "Coca-Cola"
    Entonces el producto se elimina correctamente
    Y al consultar el detalle del producto "Sprite" se encuentra
    Y al consultar el detalle de la categoría "Bebidas" se encuentra

  Escenario: No se puede eliminar un producto que no existe
    Cuando elimino un producto que no existe
    Entonces el producto no se encuentra
    Y el mensaje de error es "No se encontró el producto."
