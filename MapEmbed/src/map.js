require('./map.scss');

const $ = require('jquery');
const leaflet = require('leaflet');
const areas = require('./areas.geo.json');
let apiEndpoint = 'https://localhost:5001/api/map';
let interactions = [];
const lastYearsInteractions = [
  {
    "name": "Highland",
    "interactions": 12
  },
  {
    "name": "Outer Hebrides",
    "interactions": 6
  },
  {
    "name": "Orkney Islands",
    "interactions": 3
  },
  {
    "name": "Aberdeenshire",
    "interactions": 4
  },
  {
    "name": "Aberdeen",
    "interactions": 12
  },
  {
    "name": "Perth and Kinross",
    "interactions": 13
  },
  {
    "name": "Stirling",
    "interactions": 4
  },
  {
    "name": "Fife",
    "interactions": 15
  },
  {
    "name": "Glasgow",
    "interactions": 112
  },
  {
    "name": "Edinburgh",
    "interactions": 118
  },
  {
    "name": "Scottish Borders",
    "interactions": 26
  },
  {
    "name": "South Ayrshire",
    "interactions": 18
  },
  {
    "name": "Dumfries and Galloway",
    "interactions": 17
  }
];

window.$ = $;
window.jQuery = $;

$(document).ready(() => {
  const mapTag = $('script[src*="map.js"]');
  const mapContainer = $('<div id="purple-friday-map"></div>');
  mapContainer.insertAfter(mapTag);

  apiEndpoint = mapTag.data('endpoint') || apiEndpoint;

  const map = L.map(mapContainer[ 0 ], {
    fullscreenControl: true,
    scrollWheelZoom: false,
    trackResize: true
  }).setView([ 56.8, -4.2 ], 7);

  var osmAttrib = 'Map data &copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors, <a href="https://creativecommons.org/licenses/by-sa/2.0/">CC-BY-SA</a>';
  L.tileLayer('https://maps.wikimedia.org/osm-intl/{z}/{x}/{y}.png', {
    attribution: osmAttrib,
    minZoom: 6,
    maxZoom: 18,
    opacity: 0.8
  }).addTo(map);
  L.control.scale().addTo(map);

  $.getJSON(apiEndpoint).then(response => {
    interactions = response;
    console.log(response);
    refreshMap(map);
  }).catch(err => {
    console.error('Error getting interactions', err);
  })
});

function refreshMap(map) {
  L.geoJson(areas, { style }).addTo(map);
}

function style(feature) {
  return {
      color: '#4C0099',
      weight: 2,
      opacity: 1,
      fillColor: '#B266FF',
      fillOpacity: getOpacity(feature)
  };
}

function getOpacity(feature) {
  const area = cleanName(feature.properties.label_en);
  let oldInteractions = lastYearsInteractions.find(oldInteraction => {
    return area === cleanName(oldInteraction.name);
  });

  oldInteractions = oldInteractions ? oldInteractions.interactions : 1;

  const newInteractions = interactions.filter(newInteraction => {
    return area === cleanName(newInteraction.area)
  }).length;

  if (newInteractions > (oldInteractions * 1.5)) {
    return 0.9;
  } else if (newInteractions > oldInteractions) {
    return 0.5;
  } else if (newInteractions > 0) {
    return 0.2;
  } else {
    return 0.01;
  }
}

function cleanName(name) {
  return name.replace(/(^city of )|( city$)/ig, '');
}