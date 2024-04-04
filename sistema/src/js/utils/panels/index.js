export function configPanel(panel) {
  const panelCollapseLink = document.querySelector(`#${panel} .panel-collapse`);

  panelCollapseLink.addEventListener('click', function () {
    const panelBody = document.querySelector(`#${panel} .panel-body`);

    if (panelBody.classList.contains('in')) {
      panelBody.classList.remove('in');
    } else {
      panelBody.classList.add('in');
    }
  });
}