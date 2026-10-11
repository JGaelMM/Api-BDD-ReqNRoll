# language: es
Característica: Eliminar categorías
  Como usuario de la API
  Quiero eliminar las categorías que ya no uso
  Sin dejar productos sin categoría

  Antecedentes:
    Dado que existe una categoría con nombre "Bebidas" y descripción "Refrescos y jugos"

  Escenario: Eliminar una categoría sin productos
    Cuando elimino la categoría "Bebidas"
    Entonces la categoría se elimina correctamente
    Y al consultar el detalle de la categoría eliminada no se encuentra

  Escenario: No se puede eliminar una categoría con productos asignados
    Dado que existe un producto con nombre "Coca-Cola" en la categoría "Bebidas"
    Cuando elimino la categoría "Bebidas"
    Entonces la solicitud es rechazada por conflicto
    Y el mensaje de error es "Esta categoría no se puede eliminar porque tiene productos asignados."
    Y al consultar el detalle de la categoría "Bebidas" tiene la descripción "Refrescos y jugos"

  Escenario: Se puede eliminar una categoría después de eliminar todos sus productos
    Dado que existen los productos:
      | nombre    | descripción      | categoría |
      | Coca-Cola | Refresco de cola | Bebidas   |
      | Sprite    | Refresco de lima | Bebidas   |
    Y que elimino el producto "Coca-Cola"
    Y que elimino el producto "Sprite"
    Cuando elimino la categoría "Bebidas"
    Entonces la categoría se elimina correctamente

  Escenario: Se puede eliminar una categoría después de mover sus productos a otra categoría
    Dado que existe una categoría con nombre "Refrescos"
    Y que existe un producto con nombre "Coca-Cola" en la categoría "Bebidas"
    Y que muevo el producto "Coca-Cola" a la categoría "Refrescos"
    Cuando elimino la categoría "Bebidas"
    Entonces la categoría se elimina correctamente

  Escenario: Eliminar una categoría no afecta a las demás
    Dado que existe una categoría con nombre "Lácteos"
    Cuando elimino la categoría "Bebidas"
    Entonces la categoría se elimina correctamente
    Y al consultar el detalle de la categoría "Lácteos" se encuentra

  Escenario: No se puede eliminar una categoría que no existe
    Cuando elimino una categoría que no existe
    Entonces la categoría no se encuentra
    Y el mensaje de error es "No se encontró la categoría."
