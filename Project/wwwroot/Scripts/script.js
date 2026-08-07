const swiper = new Swiper('.slider-wrapper', {
  // Optional parameters
  
  loop: true,
  slidesPerView: 3,
  grabCursor: true,
  spaceBetween: 30,
 
  //  pagination
  pagination: {
    el: '.swiper-pagination',
    clickable: true,
    dynamicBullets: true,
  },

  // Navigation arrows
  navigation: {
    nextEl: '.swiper-button-next',
    prevEl: '.swiper-button-prev',
  },


  // Responsive breakpoints
   breakpoints: {
    0: { slidesPerView: 1 },
    768: { slidesPerView: 2 },
    992: { slidesPerView: 3 }
  }
});