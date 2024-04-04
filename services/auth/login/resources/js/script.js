document.addEventListener("DOMContentLoaded", (event) => {
  const password = document.getElementById('password')
  const btn_show_password = document.getElementById('show_password')
  const show_icon_wrapper = document.getElementById('show_icon_wrapper')
  const dont_show_icon_wrapper = document.getElementById('dont_show_icon_wrapper')

  let isShowPassword = false

  show_icon_wrapper.style.display = 'block'
  dont_show_icon_wrapper.style.display = 'none'

  btn_show_password.addEventListener('click', () => {

    isShowPassword = !isShowPassword

    if (isShowPassword) {
      password.type = 'text'
      show_icon_wrapper.style.display = 'none'

      dont_show_icon_wrapper.style.display = 'block'
    } else {
      password.type = 'password'
      show_icon_wrapper.style.display = 'block'

      dont_show_icon_wrapper.style.display = 'none'
    }
  })
})

/*!
 * Font Awesome Free 6.4.2 by @fontawesome - https://fontawesome.com
 * License - https://fontawesome.com/license/free (Icons: CC BY 4.0, Fonts: SIL OFL 1.1, Code: MIT License)
 * Copyright 2023 Fonticons, Inc.
 */