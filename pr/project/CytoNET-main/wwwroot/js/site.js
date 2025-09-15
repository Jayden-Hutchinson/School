/**
 * Rotates through banner ads at specified intervals
 * @param {number} intervalSeconds - Time in seconds between rotations
 */
function rotateBannerAds(intervalSeconds = 10) {
  const bannerAds = document.querySelectorAll('a[href="http://www.kinexusproducts.ca"] img');

  bannerAds.forEach((ad, index) => {
    ad.style.display = index === 0 ? "block" : "none";
  });

  let currentAdIndex = 0;
  const totalAds = bannerAds.length;

  setInterval(() => {
    bannerAds[currentAdIndex].style.display = "none";

    currentAdIndex = (currentAdIndex + 1) % totalAds;

    bannerAds[currentAdIndex].style.display = "block";
  }, intervalSeconds * 1000);
}

document.addEventListener("DOMContentLoaded", () => {
  rotateBannerAds(10);
});
