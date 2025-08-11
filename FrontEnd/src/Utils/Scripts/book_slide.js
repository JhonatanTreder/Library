const bookList = document.getElementById('book-list');
const arrowLeft = document.getElementById('arrow-left');
const arrowRight = document.getElementById('arrow-right');

const scrollAmount = 220;

arrowLeft.addEventListener('click', () => {
    bookList.scrollBy({left: -scrollAmount, behavior: 'smooth'});
});

arrowRight.addEventListener('click', () => {
    bookList.scrollBy({left: scrollAmount, behavior: 'smooth'});
});
