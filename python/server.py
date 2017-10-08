from flask import Flask
from flask import send_from_directory
app = Flask(__name__)

@app.route("/<path:path>")
def hello(path):
    return send_from_directory('./', path)
