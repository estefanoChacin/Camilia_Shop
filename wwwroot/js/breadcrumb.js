const breadcrumbContainer = document.getElementById("breadcrumb-container");

function updateBreadCrumb() {
    breadcrumbContainer.innerHTML = "";
    breadcrumbContainer.innerHTML += '<li class="breadcrumb-item"><a href="/">Home</a></li>'

    if(controllerName !== "Home")
    {
        breadcrumbContainer.innerHTML += '<li> class="breadcrumb-item"><a href="/${controllerName}">${controllerName}</a></li>'
        breadcrumbContainer.innerHTML+= '<li class="breadcrumb-item active" airia-current="page">${actionName}</li>'
    }
}

updateBreadCrumb();