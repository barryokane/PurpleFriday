require('./map.scss');

const $ = require('jquery');
const leaflet = require('leaflet');
const areas = require('./areas.geo.json');

window.$ = $;
window.jQuery = $;

$(document).ready(() => {
  const mapTag = $('script[src*="map.js"]');
  const mapContainer = $('<div id="purple-friday-map"></div>');
  mapContainer.insertAfter(mapTag);

  const map = L.map(mapContainer[0], {
    fullscreenControl: true,
    scrollWheelZoom: false,
    trackResize: true
  }).setView([56.4907, 4.2026], 6);

  var osmAttrib = 'Map data &copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors, <a href="https://creativecommons.org/licenses/by-sa/2.0/">CC-BY-SA</a>';
  L.tileLayer('https://maps.wikimedia.org/osm-intl/{z}/{x}/{y}.png', {
    attribution: osmAttrib,
    minZoom: 6,
    maxZoom: 18,
    opacity: 0.8
  }).addTo(map);
  L.control.scale().addTo(map);
})