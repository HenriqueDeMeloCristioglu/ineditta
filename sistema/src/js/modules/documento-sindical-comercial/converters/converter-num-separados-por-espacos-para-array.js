export function converterNumSeparadosPorEspacosParaArray(numSeparadosPorEspacos) {
  const array = numSeparadosPorEspacos?.split(" ");

  if (array[0] == "") {
    array.shift();
  }

  return array;
}
