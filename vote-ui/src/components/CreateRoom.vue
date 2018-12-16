<template>
  <v-container>
    <v-alert v-if="errors">
        {{errors}}
    </v-alert>
    <v-form v-model="valid" grid-list-lg>
      <v-text-field v-model="userName" :rules="userNameRules" :counter="20" label="User Name" required></v-text-field>
      <v-text-field v-model="roomName" :rules="roomNameRules" :counter="100" label="Room Name" required></v-text-field>
      <v-btn @click="createRoom()" color="primary" dark ripple>Create</v-btn>
    </v-form>
  </v-container>
</template>
<script>
import { apiUrl } from '../services/index.js'

export default {
  data: () => ({
    valid: false,
    errors: null,
    userName: "",
    roomName: "",
    userNameRules: [
      v => v.length <= 20 || "Name must be less 20 characters or less"
    ],
    roomNameRules: [
      v => v.length <= 100 || "Name must be less 100 characters or less"
    ],
    createRoom: function () {
        fetch(`${apiUrl}api/room`, {
            method: "POST",
            mode: "cors",
            cache: "no-cache",
            credentials: "omit", // include, *same-origin, omit
            headers: {
                "Content-Type": "application/json; charset=utf-8",
            },
            redirect: "follow", // manual, *follow, error
            body: JSON.stringify({
                name: this.roomName,
                participants: [
                    { name: this.userName }
                ]
            }), // body data type must match "Content-Type" header
        }).then(response => {
            if (response.ok) {
                response.json().then(room => {
                    this.$router.push(`/room/${room.id}`);
                });
                return;
            }
            response.text().then(errors => {
                this.errors = errors;
                console.error(errors);
            });
        }).catch(reason => {
            this.errors = reason;
            console.log('Unknown error');
        });
    }
  })
};
</script>