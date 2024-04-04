export function obterParametrosUrl() {
  const queryString = window.location.search
  return new URLSearchParams(queryString)
}
