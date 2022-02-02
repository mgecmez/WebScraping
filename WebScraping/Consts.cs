using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebScraping
{
    public static class Consts
    {
        public const string LoginCheck = @"
            (function () {
                if (document.querySelector('.header-signin') == null) {
                    console.log('user is logged in');
                } else {
                    console.log('user is not logged in');
                }
            })();
        ";

        public const string RedirectLogin = @"
            (function () {
                if (document.querySelector('.header-signin') != null) {
                    console.log('redirect login test');
                    document.querySelector('.header-signin').click();
                } else {
                    //console.log('element is null');
                    //console.log('user is logged in');
                }
            })();
        ";

        public const string Login = @"
            (function () {
                document.querySelector('#email').value = 'johngerson808@gmail.com';
                document.querySelector('#password').value = 'test8008';
                document.querySelector('.sds-button').click();
            })();
        ";

        public const string Logout = @"
            (function () {
                if (document.querySelector('.header-signout') != null) {
                    console.log('user will be logout');
                    document.querySelector('.header-signout').click();
                }
            })();
        ";

        public const string SearchOnHomePage = @"
            (function () {
                function triggerEvent(el, type) {
                    // IE9+ and other modern browsers
                    if ('createEvent' in document) {
                        var e = document.createEvent('HTMLEvents');
                        e.initEvent(type, false, true);
                        el.dispatchEvent(e);
                    } else {
                        // IE8
                        var e = document.createEventObject();
                        e.eventType = type;
                        el.fireEvent('on' + e.eventType, e);
                    }
                }

                const elmStockType = document.querySelector('#make-model-search-stocktype');
                elmStockType.value = 'used';
                triggerEvent(elmStockType, 'change');

                const elmMakes = document.querySelector('#makes');
                elmMakes.value = 'tesla';
                triggerEvent(elmMakes, 'change');

                const elmModels = document.querySelector('#models');
                elmModels.value = 'tesla-model_s';
                triggerEvent(elmModels, 'change');

                const elmPrice = document.querySelector('#make-model-max-price');
                elmPrice.value = '100000';
                triggerEvent(elmPrice, 'change');

                const elmDistance = document.querySelector('#make-model-maximum-distance');
                elmDistance.value = 'all';
                triggerEvent(elmDistance, 'change');

                const elmZipCode = document.querySelector('#make-model-zip');
                elmZipCode.value = '94596';

                const formSearch = document.querySelector('.search-form');
                formSearch.submit();

                return 'test';
            })();
        ";

        public const string GetVehicleDatas = @"
            (function () {
                var cards = document.querySelectorAll('.vehicle-card');
                var result = [];

                //var carIndexesWithHomeDelivery = [];
                //cards.forEach((item, index) => {
                //    if (item.querySelector('.vehicle-details .vehicle-badging .sds-badge--home-delivery') != null) {
                //        carIndexesWithHomeDelivery.push(index);
                //    }
                //});
                //var randomCarIndex = carIndexesWithHomeDelivery[Math.floor(Math.random() * carIndexesWithHomeDelivery.length)];

                for (var i = 0; i < cards.length; i++) {
                    const id = cards[i].id;
                    const elm = document.getElementById(id);

                    const images = [];
                    const elmImages = elm.querySelectorAll('.gallery-wrap .image-wrap > img');
                    for (var j = 0; j < elmImages.length; j++) {
                        const elmImage = elmImages[j];
                        images.push({
                            title: elmImage.alt,
                            url: elmImage.src
                        });
                    }

                    const stockType = elm.querySelector('.vehicle-details .stock-type') != null ? elm.querySelector('.vehicle-details .stock-type').innerHTML : '';
                    const title = elm.querySelector('.vehicle-details h2.title') != null ? elm.querySelector('.vehicle-details h2.title').innerHTML : '';
                    const mileage = elm.querySelector('.vehicle-details .mileage') != null ? elm.querySelector('.vehicle-details .mileage').innerHTML : '';
                    const price = elm.querySelector('.vehicle-details .primary-price') != null ? elm.querySelector('.vehicle-details .primary-price').innerHTML : '';
                    const dealerName = elm.querySelector('.vehicle-details .dealer-name strong') != null ? elm.querySelector('.vehicle-details .dealer-name strong').innerHTML : '';
                    const rating = elm.querySelector('.vehicle-details .sds-rating .sds-rating__count') != null ? elm.querySelector('.vehicle-details .sds-rating .sds-rating__count').innerHTML : '';
                    const review = elm.querySelector('.vehicle-details .sds-rating .sds-rating__link') != null ? elm.querySelector('.vehicle-details .sds-rating .sds-rating__link').innerHTML.replace('(', '').replace(')', '').replace(' reviews', '') : '';
                    const milesFrom = elm.querySelector('.vehicle-details .miles-from') != null ? elm.querySelector('.vehicle-details .miles-from').innerHTML : '';

                    //var highlights = [];
                    //if (i === randomCarIndex) {
                    //    elm.querySelector('.vehicle-details .vehicle-badging .sds-badge--home-delivery').click();
                    //    var divHighlights = document.querySelectorAll('#sds-modal ul li > div');
                    //    for (var i = 0; i < divHighlights.length; i++) {
                    //        var highlight = divHighlights[i];
                    //        var desc = highlight.getElementsByClassName('badge-description')[0].innerText;
                    //        if (highlight.className == 'home_delivery-badge') {
                    //            highlights.push({
                    //                type: 'home-delivery',
                    //                description: desc
                    //            });
                    //        }
                    //        else if (highlight.className == 'virtual_appointments-badge') {
                    //            highlights.push({
                    //                type: 'virtual-appointments',
                    //                description: desc
                    //            });
                    //        }
                    //        else if (highlight.className == 'hot_car-badge') {
                    //            highlights.push({
                    //                type: 'hot-car',
                    //                description: desc
                    //            });
                    //        }
                    //        else if (highlight.className == 'price-badge') {
                    //            var priceDetailType = highlight.getElementsByClassName('sds-badge')[0];
                    //            if (priceDetailType.className.indexOf('sds-badge-fair') > -1) {
                    //                highlights.push({
                    //                    type: 'fair-deal',
                    //                    description: desc
                    //                });
                    //            }
                    //            else if (priceDetailType.className.indexOf('sds-badge-good') > -1) {
                    //                highlights.push({
                    //                    type: 'good-deal',
                    //                    description: desc
                    //                });
                    //            }
                    //            else if (priceDetailType.className.indexOf('sds-badge-great') > -1) {
                    //                highlights.push({
                    //                    type: 'great-deal',
                    //                    description: desc
                    //                });
                    //            }
                    //        }
                    //    }
                    //    document.querySelector('#badge-modal-close').click();
                    //}

                    result.push({
                        id,
                        images,
                        stockType,
                        title,
                        mileage,
                        price,
                        dealerName,
                        rating,
                        review,
                        milesFrom
                    });
                }
                return result;
            })();
        ";

        public const string RedirectSecondPage = @"
            (function () {
                if (document.querySelector('#next_paginate') != null) {
                    document.querySelector('#next_paginate').click();
                }
            })();
        ";

        public const string ChangeSearch = @"
            (function () {
                const elmMakes = document.querySelectorAll('.sds-field.available [name=\'models[]\']')
                elmMakes[1].checked = false;
                elmMakes[2].click();
            })();
        ";
    }
}
