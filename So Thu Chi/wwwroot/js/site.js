document.addEventListener('DOMContentLoaded', function () {
    const navbar = document.querySelector('.navbar');

    function handleScroll() {
        if (window.scrollY > 0) {
            navbar.classList.add('transparent');
        } else {
            navbar.classList.remove('transparent');
        }
    }

    window.addEventListener('scroll', handleScroll);

    handleScroll();
});

document.addEventListener('DOMContentLoaded', function () {
    const dockBar = document.getElementById("sidebar").ej2_instances[0];
    document.getElementById('sidebar-toggler').onclick = function () {
        dockBar.toggle();
    };
});