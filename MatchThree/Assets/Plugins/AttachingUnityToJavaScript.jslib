mergeInto(LibraryManager.library, {

  Auth: function() {
     auth();
  },
  
  ShowAdv: function() {
    showFullscreenAdvertisement();
  },

  ShowRewardAdv: function() {
     showRewardedAdvertisement();
  },
  
  InitGame: function() {
     initGame();
  }
    
});