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

function GraphView() {
  // private
  var _sigma = null;
  var _isTemplate = true;

  function _createSigma() {
    _sigma = new sigma({
      graph: { nodes: [], edges: [] },
      container: 'graph-container',
      settings: {
        defaultNodeColor: '#006699'
      }
    });
  }

  function _preprocess(graph) {
    graph.nodes.forEach(function (el) {
      el.x = Math.random();
      el.y = Math.random();
      el.size = 15;
    }, this);
    return graph;
  }

  // public
  this.init = function () {
    _isTemplate = false;
  };

  this.loadGraph = function () {
    if (_sigma === null) _createSigma();
    _sigma.graph.clear();

    var graph = _isTemplate ? templateGraphJson : JSON.parse(arguments[0]);
    _sigma.graph.read(_preprocess(graph));
    _sigma.refresh();

    // Initialize the dragNodes plugin
    var dragListener = new sigma.plugins.dragNodes(_sigma, _sigma.renderers[0]);
  };

  this.template = function () {
    return _isTemplate;
  }
};
