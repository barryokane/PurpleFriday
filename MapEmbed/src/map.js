require('./map.scss');

const $ = require('jquery');
const leaflet = require('leaflet');
const areas = require('./areas.geo.json');

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
  debugger;
  const mapTag = $('script[src*="map.js"]');
  const apiEndpoint = mapTag.data('endpoint') || 'https://localhost:5001/api/map';
  const mapRootClass = mapTag.data('map-block-class') || 'map';
  const mapEmbedEl = $('#mapEmbed');

  const interactionPanelMarkup = '<div class="interaction-panel"></div>';
  const interactionPanelChildrenMarkup = ['<h2 class="interaction-panel__title"></h2>', '<div class="interaction-panel__tweet-list"></div>', '<p class="interaction-panel__no-data">We don\'t have any tweets from this area yet.</p>'];
  const rootContainerHtml = '<div class="' + mapRootClass + '__container"><div class="' + mapRootClass + '__map-container"></div>' + interactionPanelMarkup + '</div>';

  const interactionPanelCardInfoMarkup = '<div class="tweet-card__info"><p class="tweet-card__name"></p><p class="tweet-card__date"></p><p class="tweet-card__text"></p></div>';
  const interactionPanelCardMarkup = '<div class="tweet-card"><div class="tweet-card__image"><img /></div>' + interactionPanelCardInfoMarkup + '<a href="" target="_blank" class="tweet-card__link">View tweet</a></div>';
  const tweetCardClasses = {
    TweetName: 'tweet-card__name',
    TweetImage: 'tweet-card__image',
    TweetText: 'tweet-card__text',
    TweetDate: 'tweet-card__date',
    TweetLink: 'tweet-card__link'
  };

  if (!mapEmbedEl.length) {
    console.error('You need an element with the id #mapEmbed somewhere on the page.');
  }

  const $rootContainer = $(rootContainerHtml).appendTo(mapEmbedEl);
  const $mapContainer = $('.' + mapRootClass + '__map-container');
  const $interactionPanel = $('.interaction-panel', $rootContainer);

  $rootContainer.css({
    height: '100%',
    width: '100%'
  });

  $mapContainer.css({
    height: '100%',
    width: '100%'
  });

  const map = L.map($mapContainer[0], {
    fullscreenControl: true,
    scrollWheelZoom: false,
    trackResize: true
  }).setView([56.8, -4.2], 7);

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

    for (var markup of interactionPanelChildrenMarkup) {
      $(markup).appendTo($interactionPanel);
    }

    var $tweetList = $('.interaction-panel__tweet-list', $interactionPanel);
    if (newInteractions.length) {
      $('.interaction-panel__title').text(newInteractions[0].area);
      $('.interaction-panel__no-data').remove();
    }else {
      $('.interaction-panel__title').remove();
      $('.interaction-panel__tweet-list').remove();
    }

    for (const interaction of newInteractions) {
      const tweetCard = $(interactionPanelCardMarkup);
      $('img', '.' + tweetCardClasses.TweetImage, tweetCard)
        .attr('src', interaction.img);
      console.log(interaction);
      $('.' + tweetCardClasses.TweetName, tweetCard).text(interaction.twitterHandle || '');
      $('.' + tweetCardClasses.TweetText, tweetCard).text(interaction.text || 'text text text');
      $('.' + tweetCardClasses.TweetDate, tweetCard).text(interaction.createdDateDisplay || '');
      $('.' + tweetCardClasses.TweetLink, tweetCard).attr('href', interaction.tweetUrl || '');

      tweetCard.appendTo($tweetList);
      console.log(tweetCard);
    }

    if (!$('.interactions-panel-close', '.map__interactions-panel').length) {
      $('<button>')
        .addClass('interaction-panel__close')
        .text('x')
        .on('click', () => { $interactionPanel.toggleClass('is-active'); resetInteractionPanel(); })
        .appendTo($interactionPanel);
    }

    $interactionPanel.toggleClass('is-active');
  }

  function resetInteractionPanel() {
    $interactionPanel.empty();
  }

  function cleanName(name) {
    return name.replace(/(^city of )|( city$)/ig, '');
  }
});