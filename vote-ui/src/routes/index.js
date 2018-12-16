import VueRouter from 'vue-router';
import Home from '../components/Home';
import Room from '../components/Room';
import CreateRoom from '../components/CreateRoom';
import HelloWorld from '../components/HelloWorld'

const Routes = new VueRouter({
    routes: [
        { path: '/', component: Home  },
        { path: '/Room', component: CreateRoom },
        { path: '/Room/:roomId', component: Room },
        { path: '/HelloWorld', component: HelloWorld  }
    ]
});

export default Routes;