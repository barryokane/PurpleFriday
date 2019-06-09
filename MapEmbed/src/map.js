require('./map.scss');

const $ = require('jquery');
const leaflet = require('leaflet');
const areas = require('./areas.geo.json');
import * as Masonry from 'masonry-layout'
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
  let geojson;
  let interactions = [];

  const mapTag = $('script[src*="map.js"]');
  const apiEndpoint = mapTag.data('endpoint') || 'https://localhost:5001/api/map';
  const height = mapTag.data('height') || '100vh';
  const width = mapTag.data('width') || '100vw';

  const mapContainer = $('<div></div>')
    .css({
      'height': height,
      'width': width
    });
  mapContainer.insertAfter(mapTag);


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
    geojson = L.geoJson(areas, { style, onEachFeature }).addTo(map);
  }).catch(err => {
    console.error('Error getting interactions', err);
  })

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

  function onEachFeature(feature, layer) {
    layer.on({
        mouseover: highlightFeature,
        mouseout: resetHighlight,
        click: showInteractions
    });
  }

  function highlightFeature(e) {
    var layer = e.target;

    layer.setStyle({
        weight: 3,
        color: '#B266FF',
        fillOpacity: 0.7
    });

    if (!L.Browser.ie && !L.Browser.opera && !L.Browser.edge) {
        layer.bringToFront();
    }
  }

  function resetHighlight(e) {
    geojson.resetStyle(e.target);
  }

  function showInteractions(e) {
    const area = cleanName(e.target.feature.properties.label_en);
    const newInteractions = interactions.filter(newInteraction => {
      return area === cleanName(newInteraction.area)
    });

    const displayPanel = $('<div>')
      .addClass('interaction-panel')
      .css({
        'top': mapContainer.offset().top,
        'left': mapContainer.offset().left,
        'width': mapContainer.outerWidth(),
        'height': mapContainer.outerHeight()
      })
      .appendTo('body');

    for (const interaction of interactions) {
      const img = $('<img>')
        .addClass('interaction-image')
        .attr('src', interaction.img)
        .appendTo(displayPanel);
      console.log(img);
    }

    $('<div>')
      .addClass('interactions-panel-close')
      .css({
        'top': mapContainer.offset().top + 10,
        'left': mapContainer.outerWidth() - 25
      })
      .html('x')
      .on('click', () => { displayPanel.remove(); })
      .appendTo(displayPanel);

    var msnry = new Masonry(displayPanel[0], {
      itemSelector: '.interaction-image',
      columnWidth: 200
    });


    // let popupString = "<p style=\"text-align:center\">No messages from this area yet!<br/>Maybe <b>you</b> could be the first? Just tweet a photo with the hashtags <b>#PurpleFriday</b> and the name of the town where you took the photo (e.g. <b>#Perth</b>).</p>";
    // if (newInteractions.length > 0) {
    //     var popupInteractions = [];
    //     for (const interaction of newInteractions) {
    //         popupInteractions.push("<div><img src=\"" + interaction.img + "\"/><p style=\"float:left\"><b>" + interaction.twitterHandle + "</b></p><p style=\"float:right;margin-right:10px\"><a href=\"" + interaction.tweetUrl + "\">Link</a></p><div style=\"clear:both;\"></div></div>");
    //     }
    //     popupString = popupInteractions.join("<hr>");
    // }
    // L.popup({"maxHeight": 400, "minWidth": 300}).setContent(popupString).setLatLng(e.latlng).openOn(map);
  }

  function cleanName(name) {
    return name.replace(/(^city of )|( city$)/ig, '');
  }
});