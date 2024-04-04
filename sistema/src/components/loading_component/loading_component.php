<!DOCTYPE html>
<html>

<head>
  <!-- Your CSS and other head elements for the loading component here -->
  <style>
    /* Styling for the loading overlay */
    #loading-overlay {
      display: none;
      position: fixed;
      top: 0;
      left: 0;
      width: 100%;
      height: 100%;
      background-color: rgba(0, 0, 0, 0.5);
      z-index: 9999;
      justify-content: center;
      align-items: center;
    }

    #loading-overlay img {
      height: 4rem;
    }

    #loading-overlay #loading-div {
      display: flex;
      align-items: center;
      background-color: #f1f2f3;
      padding: 1rem;
      padding-right: 1.5rem;
    }

    #loading-overlay #loading-div span {
      font-size: 1.225rem;
      color: black;
      text-align: center;
    }
  </style>
</head>

<body>
  <div id="loading-overlay">
    <div id="loading-div">
      <img src="./assets/images/loading.svg">
      <span>Carregando...</span>
    </div>
  </div>
</body>

</html>