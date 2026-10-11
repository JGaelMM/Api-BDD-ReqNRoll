# language: es
Característica: Consultar categorías
  Como usuario de la API
  Quiero ver la lista de categorías y el detalle de cada una
  Para conocer cómo están organizados mis productos

  Escenario: Ver la lista de categorías ordenada por nombre
    Dado que existen las categorías:
      | nombre   | descripción             |
      | Limpieza | Artículos para el hogar |
      | Bebidas  | Refrescos y jugos       |
      | Lácteos  |                         |
    Cuando consulto la lista de categorías
    Entonces la lista contiene 3 categorías de un total de 3
    Y la lista muestra las categorías en este orden:
      | nombre   | descripción             |
      | Bebidas  | Refrescos y jugos       |
      | Lácteos  |                         |
      | Limpieza | Artículos para el hogar |

  Escenario: Ver la lista cuando no hay categorías
    Cuando consulto la lista de categorías
    Entonces la lista contiene 0 categorías de un total de 0

  Escenario: La lista muestra 20 categorías por página por defecto
    Dado que existen 25 categorías
    Cuando consulto la lista de categorías
    Entonces la lista contiene 20 categorías de un total de 25
    Y la lista corresponde a la página 1 con 20 categorías por página

  Esquema del escenario: Ver la lista de categorías por páginas
    Dado que existen 25 categorías
    Cuando consulto la página <página> de la lista de categorías con <tamaño> categorías por página
    Entonces la lista contiene <cantidad> categorías de un total de 25
    Y la lista corresponde a la página <página> con <tamaño> categorías por página

    Ejemplos:
      | página | tamaño | cantidad |
      | 1      | 10     | 10       |
      | 2      | 10     | 10       |
      | 3      | 10     | 5        |
      | 4      | 10     | 0        |
      | 1      | 100    | 25       |

  Esquema del escenario: No se puede consultar una página inválida
    Cuando consulto la página <página> de la lista de categorías con <tamaño> categorías por página
    Entonces la solicitud es rechazada por datos inválidos
    Y el mensaje de error es "<mensaje>"

    Ejemplos:
      | página | tamaño | mensaje                                         |
      | 0      | 10     | La página debe ser mayor o igual a 1.           |
      | 1      | 0      | El tamaño de página debe estar entre 1 y 100.   |
      | 1      | 101    | El tamaño de página debe estar entre 1 y 100.   |

  Escenario: Ver el detalle de una categoría
    Dado que existe una categoría con nombre "Bebidas" y descripción "Refrescos y jugos"
    Cuando consulto el detalle de la categoría "Bebidas"
    Entonces la categoría se encuentra
    Y la categoría consultada tiene el nombre "Bebidas"
    Y la categoría consultada tiene la descripción "Refrescos y jugos"

  Escenario: No se puede ver el detalle de una categoría que no existe
    Cuando consulto el detalle de una categoría que no existe
    Entonces la categoría no se encuentra
    Y el mensaje de error es "No se encontró la categoría."
