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

$.throttle = jq_throttle = function( delay, no_trailing, callback, debounce_mode ) {
  // After wrapper has stopped being called, this timeout ensures that
  // `callback` is executed at the proper times in `throttle` and `end`
  // debounce modes.
  var timeout_id,
    
    // Keep track of the last time `callback` was executed.
    last_exec = 0;
  
  // `no_trailing` defaults to falsy.
  if ( typeof no_trailing !== 'boolean' ) {
    debounce_mode = callback;
    callback = no_trailing;
    no_trailing = undefined;
  }
  
  // The `wrapper` function encapsulates all of the throttling / debouncing
  // functionality and when executed will limit the rate at which `callback`
  // is executed.
  function wrapper() {
    var that = this,
      elapsed = +new Date() - last_exec,
      args = arguments;
    
    // Execute `callback` and update the `last_exec` timestamp.
    function exec() {
      last_exec = +new Date();
      callback.apply( that, args );
    };
    
    // If `debounce_mode` is true (at_begin) this is used to clear the flag
    // to allow future `callback` executions.
    function clear() {
      timeout_id = undefined;
    };
    
    if ( debounce_mode && !timeout_id ) {
      // Since `wrapper` is being called for the first time and
      // `debounce_mode` is true (at_begin), execute `callback`.
      exec();
    }
    
    // Clear any existing timeout.
    timeout_id && clearTimeout( timeout_id );
    
    if ( debounce_mode === undefined && elapsed > delay ) {
      // In throttle mode, if `delay` time has been exceeded, execute
      // `callback`.
      exec();
      
    } else if ( no_trailing !== true ) {
      // In trailing throttle mode, since `delay` time has not been
      // exceeded, schedule `callback` to execute `delay` ms after most
      // recent execution.
      // 
      // If `debounce_mode` is true (at_begin), schedule `clear` to execute
      // after `delay` ms.
      // 
      // If `debounce_mode` is false (at end), schedule `callback` to
      // execute after `delay` ms.
      timeout_id = setTimeout( debounce_mode ? clear : exec, debounce_mode === undefined ? delay - elapsed : delay );
    }
  };
  
  // Set the guid of `wrapper` function to the same of original callback, so
  // it can be removed in jQuery 1.4+ .unbind or .die by using the original
  // callback as a reference.
  if ( $.guid ) {
    wrapper.guid = callback.guid = callback.guid || $.guid++;
  }
  
  // Return the wrapper function.
  return wrapper;
};

$(document).ready(() => {
  let geojson;
  let interactions = [];
  //debugger;
  const mapTag = $('script[src*="map.js"]');
  const apiEndpoint = mapTag.data('endpoint');
  const mapRootClass = mapTag.data('map-block-class') || 'map';
  const mapEmbedEl = $('#mapEmbed');

  const interactionPanelMarkup = '<div class="interaction-panel cleanslate"></div>';
  const interactionPanelChildrenMarkup = ['<h2 class="interaction-panel__title cleanslate"></h2>', '<div class="interaction-panel__search cleanslate"><label><span class="label-text">Search for a twitter handle or tweet text</span><input class="js-interaction-panel-search" type="text" val="" /></label><button class="interaction-panel__search-clear js-interaction-panel-clear-search">Clear search</button><span class="interaction-panel__search-info js-search-info-results"></span></div>', '<div class="interaction-panel__tweet-list"></div>', '<p class="interaction-panel__no-data">We don\'t have any tweets from this area yet.</p>'];
  const rootContainerHtml = '<div class="' + mapRootClass + '__container"><div class="' + mapRootClass + '__map-container"></div>' + interactionPanelMarkup + '</div>';

  const interactionPanelCardInfoMarkup = '<div class="tweet-card__info cleanslate"><p class="tweet-card__name"></p><p class="tweet-card__date"></p><p class="tweet-card__text"></p></div>';
  const interactionPanelCardMarkup = '<div class="tweet-card cleanslate"><div class="tweet-card__image"><img /></div>' + interactionPanelCardInfoMarkup + '<a href="" target="_blank" class="tweet-card__link">View tweet</a></div>';
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

  var currentTweetList = null;
  function showInteractions(e) {
    const area = cleanName(e.target.feature.properties.label_en);
    const newInteractions = interactions.filter(newInteraction => {
      return area === cleanName(newInteraction.area)
    });

    currentTweetList = newInteractions;
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
      $('.interaction-panel__search').remove();
    }

    for (const interaction of newInteractions) {
      const tweetCard = $(interactionPanelCardMarkup);
      $('img', tweetCard)
        .attr('src', interaction.img);
      $('.' + tweetCardClasses.TweetName, tweetCard).text(interaction.twitterHandle || '');
      $('.' + tweetCardClasses.TweetText, tweetCard).text(interaction.text || 'text text text');
      $('.' + tweetCardClasses.TweetDate, tweetCard).text(interaction.createdDateDisplay || '');
      $('.' + tweetCardClasses.TweetLink, tweetCard).attr('href', interaction.tweetUrl || '');

      tweetCard.appendTo($tweetList);
    }

    if (!$('.interactions-panel-close', '.map__interactions-panel').length) {
      $('<button>')
        .addClass('interaction-panel__close')
        .text('x')
        .on('click', () => { $interactionPanel.toggleClass('is-active'); resetInteractionPanel(); })
        .appendTo($interactionPanel);
    }

    $('.interaction-panel__search').on('keyup change', $.throttle(300, function(e){
      // var searchRegex = new RegExp($('.js-interaction-panel-search').val(), 'gi');
      var normalizedSearch = $('.js-interaction-panel-search').val().toLowerCase();
      var results = currentTweetList.filter(function(tweet){
        // return searchRegex.test(tweet.twitterHandle) || searchRegex.test(tweet.text);
        return tweet.twitterHandle.toLowerCase().includes(normalizedSearch) || tweet.text.toLowerCase().includes(normalizedSearch);
      });

      $('.interaction-panel__tweet-list').empty();

      for (const interaction of results) {
        const tweetCard = $(interactionPanelCardMarkup);
        $('img', tweetCard)
          .attr('src', interaction.img);
        $('.' + tweetCardClasses.TweetName, tweetCard).text(interaction.twitterHandle || '');
        $('.' + tweetCardClasses.TweetText, tweetCard).text(interaction.text || 'text text text');
        $('.' + tweetCardClasses.TweetDate, tweetCard).text(interaction.createdDateDisplay || '');
        $('.' + tweetCardClasses.TweetLink, tweetCard).attr('href', interaction.tweetUrl || '');
  
        tweetCard.appendTo($tweetList);
      }
      
      var resultsText = results.length + (results.length > 1 || results.length !== 0? ' results': ' result') + ' found';
      $('.js-search-info-results').text(resultsText);
    }));

    $('.js-interaction-panel-clear-search').on('click', function(){
      $('.js-interaction-panel-search').val('');
      $('.js-interaction-panel-search').trigger('keyup');
    });

    $interactionPanel.toggleClass('is-active');
  }

  function resetInteractionPanel() {
    $interactionPanel.empty();
    currentTweetList = null;
  }

  function cleanName(name) {
    return name.replace(/(^city of )|( city$)/ig, '');
  }
});