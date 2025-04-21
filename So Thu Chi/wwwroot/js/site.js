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