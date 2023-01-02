function test() {
    width = 1000;
    const svg = d3.create('svg').attr('viewBox', [0, 0, width, 400]);

    // Define variables for your grid.
    const gridSize = 25;
    const gridDotSize = 3;
    const gridColor = '#2e2e2e';

    // Create a pattern element, and give it an id to reference later.
    var pattern = svg.append('pattern')
        .attr('id', 'grid-pattern')
        .attr('patternUnits', 'userSpaceOnUse')
        .attr('x', 0)
        .attr('y', 0)
        .attr('width', gridSize)
        .attr('height', gridSize);
    pattern.append('line')
        .attr('stroke', gridColor)
        .attr('x1', 0)
        .attr('y1', 0)
        .attr('x2', gridSize * 16)
        .attr('y2', 0);
    pattern.append('line')
        .attr('stroke', gridColor)
        .attr('x1', 0)
        .attr('y1', 0)
        .attr('x2', 0)
        .attr('y2', gridSize * 16);

    // Append a "backdrop" rect to your svg, and fill it with your pattern.
    // You shouldn't need to touch this again after adding it.
    svg.append('rect')
        .attr('fill', 'url(#grid-pattern)')
        .attr('width', '100%')
        .attr('height', '100%');

    // Call this function to modify the pattern when zooming/panning.
    function updateGrid(zoomEvent) {
        svg.select('#grid-pattern')
            .attr('x', zoomEvent.transform.x)
            .attr('y', zoomEvent.transform.y)
            .attr('width', gridSize * zoomEvent.transform.k)
            .attr('height', gridSize * zoomEvent.transform.k)
            .selectAll('line')
            .attr('opacity', Math.min(zoomEvent.transform.k, 1)); // Lower opacity as the pattern gets more dense.
    }

    // Place all svg content inside of a group, adjacent to the backdrop rect.
    var content = svg.append('g').attr('id', 'content');

    // Example content.
    content.append('rect').attr('fill', '#65D097').attr('rx', 10).attr('width', 100).attr('height', 100).attr('x', width / 2 - 75).attr('y', 125);
    content.append('rect').attr('fill', '#75E6CF').attr('rx', 5).attr('width', 50).attr('height', 50).attr('x', width / 2 + 35).attr('y', 175);
    content.append('rect').attr('fill', '#5CC4FF').attr('rx', 3).attr('width', 25).attr('height', 25).attr('x', width / 2 + 35).attr('y', 235);

    // Call d3.zoom to enable zooming/panning in this svg.
    svg.call(d3.zoom()
        .scaleExtent([0.25, 2])
        .on("zoom", (event) => {
            content.attr('transform', event.transform);
            updateGrid(event); // We need to update the grid with every zoom event.
        }));

    return svg.node();
}