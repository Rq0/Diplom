class Hello extends React.Component {
    render() {
        return <div>
		<h1>Привет, React.JS</h1> 
		<button className="btn btn-warning">Удалить</button>
		</div>;
    }
}
ReactDOM.render(
    <Hello />,
    document.getElementById("content")
);