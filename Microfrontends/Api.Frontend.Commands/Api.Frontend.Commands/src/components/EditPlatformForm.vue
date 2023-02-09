<template>
  <b-form class="mt-3">
    <b-row>
      <b-row>
        <h4 class="text-secondary">Platform Details</h4>
      </b-row>
      <b-col cols="6">
        <b-form-group id="name" label="Name" label-for="name">
          <b-form-input
            id="name"
            type="text"
            placeholder="Name"
            v-model="platform.name"
          ></b-form-input>
        </b-form-group>
      </b-col>
      <b-col cols="6">
        <b-form-group id="publisher" label="Publisher" label-for="publisher">
          <b-form-input
            id="publisher"
            type="text"
            placeholder="Publisher"
            v-model="platform.publisher"
          ></b-form-input>
        </b-form-group>
      </b-col>
    </b-row>
    <b-row class="mt-3">
      <b-col cols="6">
        <b-form-group id="cost" label="Cost" label-for="cost">
          <b-form-input
            id="email"
            type="text"
            placeholder="Cost"
            v-model="platform.cost"
          ></b-form-input>
        </b-form-group>
      </b-col>
    </b-row>
    <b-row class="mt-4">
      <b-col cols="3">
        <b-button variant="primary" class="px-5" @click="updatePlatform"
          >Update Platform</b-button
        >
      </b-col>
      <b-col>
        <b-button variant="warning" @click="triggerClose">Close</b-button>
      </b-col>
    </b-row>
  </b-form>
</template>

<script>
import axios from "axios";

export default {
  name: "EditPlatformModal",
  props: {
    platformId: Number,
  },
  data() {
    return {
      platform: {},
    };
  },
  mounted() {
    this.getPlatformByID();
  },
  methods: {
    triggerClose() {
      this.$emit("closeEditModal");
    },
    getPlatformByID() {
      axios
        .get(`http://soa-project.com/api/platforms/${this.platformId}`)
        .then((response) => {
          this.platform = response.data;
        })
        .catch((error) => {
          console.log(error);
        });
    },
    updatePlatform() {
      axios
        .post(
          `http://soa-project.com/api/platforms/${this.platformId}`,
          this.platform,
          {
            headers: {
              "Content-Type": "application/json",
            },
          }
        )
        .then((response) => {
          console.log(response.data);
          this.$emit("closeEditModal");
          this.$emit("reloadDataTable");
          this.$emit("showSuccessAlert");
        })
        .catch((error) => {
          console.log(error);
        });
    },
  },
};
</script>
