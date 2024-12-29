import http from 'k6/http';
import { sleep } from 'k6';

export const options = {
  iterations: 10000,
  vus: 200
};

export default function () {
    let data = JSON.stringify({ animal: "furry cats!!"});
    let headers = {
      'Content-Type': 'application/json',
      "Host": "www.example.com"
    };

    http.post('http://localhost:8080/ratings', data, {headers: headers});
  
    sleep(0.3);
  }