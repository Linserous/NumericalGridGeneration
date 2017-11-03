var templateGraphJson = {
  "edges": [{ id: 'n1', source: 'n1', target: 'n2' },
  { id: 'n2', source: 'n1', target: 'n3' },
  { id: 'n3', source: 'n2', target: 'n3' },
  { id: 'n4', source: 'n4', target: 'n5' }],
  "nodes": [{ id: 'n1', label: '1', size: 10, x: Math.random(), y: Math.random() },
  { id: 'n2', label: '2', size: 10, x: Math.random(), y: Math.random() },
  { id: 'n3', label: '3', size: 10, x: Math.random(), y: Math.random() },
  { id: 'n4', label: '4', size: 10, x: Math.random(), y: Math.random() },
  { id: 'n5', label: '5', size: 10, x: Math.random(), y: Math.random() }]
};

var graphView = new GraphView();

window.onload = function (e) {
  if (graphView.template()) graphView.loadGraph();
};

function init() {
  graphView.init();
}

function loadGraph(jsonStr) {
  graphView.loadGraph(jsonStr);
}

function render(isRender) {
  graphView.render(isRender);
}

function isRendered() {
  return graphView.isRendered();
}

function notify(message, type) {
  var handlers = ['confirm', 'alert', 'alert'];
  if (type >= handlers.length) return;
  window[handlers[type]](message);
}

function GraphView() {
  // private
  var _sigma1 = null;
  var _sigma2 = null;
  var _isTemplate = true;

  function _createSigma(container, graph, settings) {
    graph = graph != undefined ? graph : { nodes: [], edges: [] };
    settings = settings != undefined ? settings : {};
    var s = new sigma({
      graph: graph,
      container: container,
      settings: Object.assign({
        skipErrors: true,
        labelThreshold: 1
      }, settings)
    });
    // Initialize the dragNodes plugin
    var dragListener = new sigma.plugins.dragNodes(s, s.renderers[0]);
    return s;
  }

  function _preprocess(graph, callback) {
    callback = callback != undefined ? callback : function () { };
    graph.nodes.forEach(function (el) {
      callback(el);
      el.x = 'x' in el ? el.x : Math.random();
      el.y = 'y' in el ? el.y : Math.random();
      el.size = 1;

    }, this);
    return graph;
  }

  // public
  this.init = function () {
    _isTemplate = false;
  };

  this.loadGraph = function () {
    if (_sigma1 != null && _sigma2 != null) {
      _sigma1.graph.clear();
      _sigma2.graph.clear();
    }

    var graph1 = _isTemplate ? templateGraphJson : JSON.parse(arguments[0]);
    if (_sigma1 == null) {
      _sigma1 = _createSigma('graph1',
        _preprocess(graph1),
        { defaultNodeColor: '#006699' });
    } else {
      _sigma1.graph.read(_preprocess(graph1));
    }

    var graph2 = _isTemplate ? templateGraphJson : JSON.parse(arguments[0]);
    if (_sigma2 == null) {
      _sigma2 = _createSigma('graph2',
        _preprocess(graph2));
    } else {
      _sigma2.graph.read(_preprocess(graph2));
    }

    _sigma1.refresh();
    _sigma2.refresh();

    render(true);
    setTimeout(function () { render(false); }, 500);
  };

  this.template = function () {
    return _isTemplate;
  }

  this.render = function (isRender) {
    if (_sigma1 == null) return;

    if (isRender) {
      if (_sigma1.isForceAtlas2Running()) _sigma1.killForceAtlas2();
      // Start the layout algorithm
      _sigma1.startForceAtlas2({
        linLogMode: false,
        slowDown: 1,
        worker: false,
        barnesHutOptimize: false
      });
    } else if (_sigma1.isForceAtlas2Running()) {
      _sigma1.killForceAtlas2();
    }
  }

  this.isRendered = function () {
    return _sigma1.isForceAtlas2Running();
  }
};

