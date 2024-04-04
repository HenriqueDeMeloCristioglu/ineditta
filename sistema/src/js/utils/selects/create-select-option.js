export async function createSelectOption(data) {
  const options = []

  data.map(({ value, description }) => {
    options.push({
      id: value,
      description
    })
  })

  return await Promise.resolve(options)
}