import http from 'k6/http';
import { sleep } from 'k6';

export const options = {
  iterations: 200_000,
  vus: 1400
};

export default function () {
    // let data = createData();
    let headers = {
      'Content-Type': 'application/json',
      "Host": "www.example.com"
    };

    // http.post('http://localhost:8080/api/ratings', data, {headers: headers});
    http.get('http://localhost:8080/api/ratings', {headers: headers});
  
    sleep(0.1);
  }

  function createData() {
    return JSON.stringify({
      items: [
        { 
          username: "DogMaster"
        }
      ]
    });
  }